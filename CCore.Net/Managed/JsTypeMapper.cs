﻿using CCore.Net.JsRt;
using CCore.Net.Managed.Mapping;
using CCore.Net.Runtimes;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CCore.Net.Managed
{
    public static class JsTypeMapper
    {
        public static bool MapObjectsAutomatically { get; set; } = false;
        public static bool MapTypesAutomatically { get; set; } = false;
        public static MappingValidator MappingValidator { get; set; } = new MaxMappingValidator();

        public static JsValueRef ToScript(object obj)
        {
            if (obj == null)
                return JsValueRef.Null;

            if(obj is JsValueRef jsRef) // We trying to map same to same, so just pass them trougth
                return jsRef;
            if(obj is JsValue jsVal)
                return jsVal;


            if(obj is string str)
                return new JsString(str);
            if(obj is bool b)
                return new JsBool(b);
            if(obj is int intVal)
                return JsNumber.FromNumber(intVal);
            if(obj is long longVal)
                return JsNumber.FromNumber(longVal);
            if(obj is float floatVal)
                return JsNumber.FromNumber(floatVal);
            if(obj is double doubleVal)
                return JsNumber.FromNumber(doubleVal);
            if (obj is decimal decimalVal)
                return JsNumber.FromNumber(decimalVal);

            if (obj is Delegate @delegate)
                return JsManagedFunction.ObtainUsingDelegate(@delegate);


            if (MapObjectsAutomatically)
                return JsManagedObject.Obtain(obj, MappingValidator);


            throw new ArgumentException("Unsupported type to be converted to script.");
        }

        public static object ToHost(JsValueRef jsValue, Type expectedType, out bool success)
        {
            success = true;
            var actualType = jsValue.ValueType;
            if (expectedType == typeof(JsValueRef))
                return jsValue;

            if (jsValue.Equals(JsValueRef.Null))
                return null;

            if (expectedType == typeof(string) && JsString.IsSupported(actualType, jsValue))
                return (string)new JsString(jsValue); 
            if (expectedType == typeof(int) && JsNumber.IsSupported(actualType, jsValue))
                return (int)new JsNumber(jsValue);
            if (expectedType == typeof(long) && JsNumber.IsSupported(actualType, jsValue))
                return (long)new JsNumber(jsValue);
            if (expectedType == typeof(float) && JsNumber.IsSupported(actualType, jsValue))
                return (float)new JsNumber(jsValue);
            if (expectedType == typeof(decimal) && JsNumber.IsSupported(actualType, jsValue))
                return (decimal)new JsNumber(jsValue);
            if (expectedType == typeof(double) && JsNumber.IsSupported(actualType, jsValue))
                return (double)new JsNumber(jsValue);
            if (expectedType == typeof(bool) && JsBool.IsSupported(actualType, jsValue))
                return (bool)new JsBool(jsValue);

            var value = FromRaw(jsValue);
            if (typeof(JsValue).IsAssignableFrom(expectedType))
            {
                if (expectedType.IsAssignableFrom(value.GetType()))
                {
                    return value; // This code smells
                }
                success = false;
                return null;
            }
            if (value is JsManagedObject managedObject)
            {
                if (expectedType.IsAssignableFrom(managedObject.Target.GetType()))
                    return managedObject.Target;
            }
            success = false;
            return null;
        }

        public static JsValue FromRaw(JsValueRef jsValue)
        {
            if (!jsValue.IsValid)
                throw new Exception("Invalid value");
            var type = jsValue.ValueType;
            switch (type)
            {
                case JsValueType.Undefined:
                    return new JsUndefined(jsValue);
                case JsValueType.Null:
                    return new JsNull(jsValue);
                case JsValueType.Number:
                    return new JsNumber(jsValue);
                case JsValueType.String:
                    return new JsString(jsValue);
                case JsValueType.Boolean:
                    return new JsBool(jsValue);
                case JsValueType.Object:
                    if(jsValue.HasExternalData)
                    {
                        var handle = GCHandle.FromIntPtr(jsValue.ExternalData);
                        if (handle.Target is JsManagedObject managedObject)
                            return managedObject;
                    }
                    return new JsObject(jsValue);
                case JsValueType.Function:
                    var externalObj = jsValue.GetIndexedProperty(JsValueRef.From(JsValue.ExternalObjectPropertyName));
                    if(externalObj.HasExternalData) // Recover managed objects
                    {
                        var handle = GCHandle.FromIntPtr(externalObj.ExternalData);
                        if (handle.Target is JsManagedFunction managedFunction)
                            return managedFunction;
                        if(handle.Target is JsNativeFunction nativeFunction)
                            return nativeFunction;
                        throw new Exception("Unsupprted managed object");
                    }
                    return new JsFunction(jsValue);
                case JsValueType.Error:
                    return new JsError(jsValue);
                case JsValueType.Array:
                    return new JsArray(jsValue);
                case JsValueType.Symbol:
                case JsValueType.ArrayBuffer:
                case JsValueType.TypedArray:
                case JsValueType.DataView:
                    return new JsObject(jsValue); // They are close enougth to objects
                default:
                    throw new ArgumentOutOfRangeException();
            }
            throw new NotImplementedException("Unsupported value");
        }
    }
}
