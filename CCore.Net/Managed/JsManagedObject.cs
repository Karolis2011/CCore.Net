using CCore.Net.JsRt;
using CCore.Net.Managed.Mapping;
using CCore.Net.Runtimes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace CCore.Net.Managed
{
    public class JsManagedObject : JsObject, IJsFreeable
    {
        public object Target { get; private set; }
        protected JsFinalizeCallback finalizeCallback;
        public bool IsFreeed { get; protected set; } = false;

        protected GCHandle finalizeCallbackHandle;
        protected GCHandle selfHandle;

        public static JsManagedObject Obtain(object obj, MappingValidator validator = null)
        {
            var runtime = BasicJsRuntime.ActiveRuntime;
            if (runtime == null)
                throw new InvalidOperationException("No runtime present");
            if (runtime.TryGetExistingManaged(obj, out var value))
            {
                if (value is JsManagedObject managedObject && obj.GetType() == managedObject.Target.GetType())
                    return managedObject;
            }
            return new JsManagedObject(obj, validator);
        }

        internal JsManagedObject(object obj, MappingValidator validator = null) : base()
        {
            if(obj == null)
                throw new ArgumentNullException(nameof(obj));
            Target = obj;
            finalizeCallback = (ptr) =>
                OnJsFinalize();
            finalizeCallbackHandle = GCHandle.Alloc(finalizeCallback);
            selfHandle = GCHandle.Alloc(this);
            jsValueRef = JsValueRef.CreateExternalObject(IntPtr.Zero, finalizeCallback);
            jsValueRef.ExternalData = (IntPtr)selfHandle;

            if (validator == null)
                validator = new MaxMappingValidator();
            var mappingInfo = validator.Map(Target.GetType());
            if (!mappingInfo.Mapped)
                throw new InvalidOperationException("This object's mapping was forbidden.");
            ProjectFields(validator);
            ProjectProperties(validator);
            ProjectMethods(validator);

            if (mappingInfo.Freeze)
                Freeze();

            runtime.TrackManaged(this, obj);
        }

        private BindingFlags GetBindingFlags() => BindingFlags.Public | BindingFlags.Instance;

        private void ProjectFields(MappingValidator validator)
        {
            var type = Target.GetType();
            var bindingFlags = GetBindingFlags();

            var fields = type.GetFields(bindingFlags);

            foreach (var field in fields)
            {
                var mappingInfo = validator.Map(type, field);
                if (!mappingInfo.Mapped)
                    continue;

                var getter = new JsNativeFunction((callee, isConstructCall, args, argCount, callbackData) =>
                {
                    try
                    {
                        var result = field.GetValue(Target);
                        return JsTypeMapper.ToScript(result);
                    }
                    catch (Exception e)
                    {
                        e = JsManagedUtils.UnwrapException(e);
                        if (e is JsException jsrte)
                        {
                            runtime.SetException(JsManagedUtils.CreateErrorFromWrapperException(jsrte));
                        }
                        else
                        {
                            runtime.SetException(new JsError($"Error ocured while reading field '{mappingInfo.Name}': {e.Message}"));
                        }
                    }
                    return JsUndefined.Undefined;
                }, IntPtr.Zero);

                var setter = new JsNativeFunction((callee, isConstructCall, args, argCount, callbackData) =>
                {
                    try
                    {
                        field.SetValue(Target, JsTypeMapper.ToHost(args[1], field.FieldType));
                    }
                    catch (Exception e)
                    {
                        e = JsManagedUtils.UnwrapException(e);
                        if (e is JsException jsrte)
                        {
                            runtime.SetException(JsManagedUtils.CreateErrorFromWrapperException(jsrte));
                        }
                        else
                        {
                            runtime.SetException(new JsError($"Failed to set value to field '{mappingInfo.Name}': {e.Message}"));
                        }
                    }
                    return JsUndefined.Undefined;
                }, IntPtr.Zero);

                var descriptor = NewObject();
                descriptor["enumerable"] = (JsBool)mappingInfo.Enumerable;
                descriptor["configurable"] = (JsBool)!mappingInfo.Freeze;
                descriptor["get"] = getter;
                descriptor["set"] = setter;
                DefineProperty((JsString)mappingInfo.Name, descriptor);
            }
        }

        private void ProjectProperties(MappingValidator validator)
        {
            var type = Target.GetType();
            var bindingFlags = GetBindingFlags();

            var properties = type.GetProperties(bindingFlags);

            foreach (var property in properties)
            {
                var mappingInfo = validator.Map(type, property);
                if (!mappingInfo.Mapped)
                    continue;

                var descriptor = NewObject();
                descriptor["enumerable"] = (JsBool)mappingInfo.Enumerable;
                descriptor["configurable"] = (JsBool)!mappingInfo.Freeze;
                if (property.GetGetMethod() != null)
                {
                    var getter = new JsNativeFunction((callee, isConstructCall, args, argCount, callbackData) =>
                    {
                        try
                        {
                            var result = property.GetValue(Target);
                            return JsTypeMapper.ToScript(result);
                        }
                        catch (Exception e)
                        {
                            e = JsManagedUtils.UnwrapException(e);
                            if (e is JsException jsrte)
                            {
                                runtime.SetException(JsManagedUtils.CreateErrorFromWrapperException(jsrte));
                            }
                            else
                            {
                                runtime.SetException(new JsError($"Property '{mappingInfo.Name}' get failed: {e.Message}"));
                            }
                        }
                        return JsUndefined.Undefined;
                    }, IntPtr.Zero);
                    descriptor["get"] = getter;
                }
                if (property.GetSetMethod() != null)
                {
                    var setter = new JsNativeFunction((callee, isConstructCall, args, argCount, callbackData) =>
                    {
                        try
                        {
                            property.SetValue(Target, JsTypeMapper.ToHost(args[1], property.PropertyType));
                        }
                        catch (Exception e)
                        {
                            e = JsManagedUtils.UnwrapException(e);
                            if (e is JsException jsrte)
                            {
                                runtime.SetException(JsManagedUtils.CreateErrorFromWrapperException(jsrte));
                            }
                            else
                            {
                                runtime.SetException(new JsError($"Property '{mappingInfo.Name}' set failed: {e.Message}"));
                            }
                        }
                        return JsUndefined.Undefined;
                    }, IntPtr.Zero);
                    descriptor["set"] = setter;
                }
                DefineProperty((JsString)mappingInfo.Name, descriptor);
            }
        }

        private void ProjectMethods(MappingValidator validator)
        {
            var type = Target.GetType();
            var bindingFlags = GetBindingFlags();

            var methodGroups = type.GetMethods(bindingFlags)
                .Select((m) => new { m, i = validator.Map(type, m) })
                .Where((m) => m.i.Mapped)
                .GroupBy((m) => m.i.Name);

            foreach (var methodGroup in methodGroups)
            {
                string name = methodGroup.Key;
                var method = new JsManagedMethod(methodGroup.Select((m) => m.m).ToArray(), Target, name);

                var mappingInfo = methodGroup.First().i;
                var descriptor = NewObject();
                descriptor["enumerable"] = (JsBool)mappingInfo.Enumerable;
                descriptor["configurable"] = (JsBool)!mappingInfo.Freeze;
                descriptor["value"] = method;
                DefineProperty((JsString)mappingInfo.Name, descriptor);
            }
        }

        protected void OnJsFinalize()
        {
            if (!IsFreeed)
            {
                finalizeCallbackHandle.Free();
                selfHandle.Free();
                IsFreeed = true;
            }
        }
    }
}
