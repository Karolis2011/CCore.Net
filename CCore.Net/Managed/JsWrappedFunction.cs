using CCore.Net.JsRt;
using CCore.Net.Runtimes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CCore.Net.Managed
{
    public class JsWrappedFunction : JsNativeFunction
    {
        protected JsRt.JsNativeFunction WrapDelegate(Delegate @delegate)
        {
            return (callee, isConstructCall, args, argCount, callbackData) =>
            {
                var methodInfo = @delegate.GetMethodInfo();
                var parameters = methodInfo.GetParameters();
                try
                {
                    if (!IsCompatibleSignature(args.Select(v => v.ValueType).ToArray(), args, parameters, out object[] processedArgs))
                    {
                        throw new Exception("Method signature is incompatible with passed arguments.");
                    }
                    object result = @delegate.DynamicInvoke(processedArgs);

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
                        runtime.SetException(new JsError($"Error invoking managed function: {e.Message}"));
                    }
                }
                return JsValueRef.Undefined;
            };
        }

        protected JsRt.JsNativeFunction WrapDelegate(MethodInfo[] methodGroup, object obj)
        {
            return (callee, isConstructCall, args, argCount, callbackData) =>
            {
                try
                {
                    if (!IsCompatibleSignature(methodGroup, args, out MethodInfo bestSelection, out object[] processedArgs))
                    {
                        throw new Exception("Method signature is incompatible with passed arguments.");
                    }
                    object result = bestSelection.Invoke(obj, processedArgs);

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
                        runtime.SetException(new JsError($"Error invoking managed function: {e.Message}"));
                    }
                }
                return JsValueRef.Undefined;
            };
        }

        private bool IsCompatibleSignature(MethodInfo[] methods, JsValueRef[] args, out MethodInfo bestSelection, out object[] processedArgs)
        {
            var argTypes = args.Select(v => v.ValueType).ToArray();
            var preProcessed = methods.Select((m) => new { m, p = m.GetParameters() }).OrderByDescending((c) => c.p.Length);
            foreach (var cand in preProcessed)
            {
                if (IsCompatibleSignature(argTypes, args, cand.p, out processedArgs))
                {
                    bestSelection = cand.m;
                    return true;
                }
            }
            bestSelection = null;
            processedArgs = null;
            return false;
        }

        private bool IsCompatibleSignature(JsValueType[] argTypes, JsValueRef[] args, ParameterInfo[] parameters, out object[] mappedParameters)
        {
            mappedParameters = new object[parameters.Length];
            var mp = mappedParameters;
            int argPos = 1; //Start 

            var globalTypeSwitch = new Dictionary<Type, Action<int>>()
            {
                { typeof(JsContext), (i) => mp[i] = runtime.InternalContext },
                { typeof(IJsRuntime), (i) => mp[i] = runtime },
                { typeof(BasicJsRuntime), (i) => mp[i] = runtime },
            };


            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                if (globalTypeSwitch.ContainsKey(param.ParameterType))
                {
                    globalTypeSwitch[param.ParameterType](i);
                }
                else
                {
                    if (argPos == args.Length)
                        return false;
                    try
                    {
                        mp[i] = JsTypeMapper.ToHost(args[argPos], param.ParameterType, out var success);
                        if(!success)
                            return false;
                        argPos++;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }

            }
            mappedParameters = mp;
            return true;
        }
    }
}
