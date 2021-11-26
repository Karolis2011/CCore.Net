using CCore.Net.JsRt;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.Net.Managed
{
    public static class JsTypeMapper
    {


        public static JsValueRef ToScript(object obj)
        {
            if (obj == null)
                return JsValueRef.Null;

            if(obj is JsValueRef jsRef) // We trying to map same to same, so just pass them trougth
                return jsRef;
            if(obj is JsValue jsVal) // 
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
                return new JsManagedFunction(@delegate);


            throw new ArgumentException("Unsupported type to be converted to script.");
        }

        public static object ToHost(JsValueRef jsValue, Type expectedType)
        {
            var actuallType = jsValue.ValueType;
            if (typeof(JsValue).IsAssignableFrom(expectedType))
            {
                var value = FromRaw(jsValue);
                if (expectedType.IsAssignableFrom(value.GetType())) {
                    return value; // This code smells
                } else
                {
                    throw new Exception("Type missmatch.");
                }
            }
            if (jsValue.Equals(JsValueRef.Null))
                return null;

            if (expectedType == typeof(string))
                return (string)new JsString(jsValue);
            if (expectedType == typeof(int))
                return (int)new JsNumber(jsValue);
            if (expectedType == typeof(long))
                return (long)new JsNumber(jsValue);
            if (expectedType == typeof(float))
                return (float)new JsNumber(jsValue);
            if (expectedType == typeof(decimal))
                return (decimal)new JsNumber(jsValue);
            if (expectedType == typeof(double))
                return (double)new JsNumber(jsValue);
            if (expectedType == typeof(bool))
                return (bool)new JsBool(jsValue);

            throw new ArgumentException("Unsupported type to be converted to host.");
        }

        public static JsValue FromRaw(JsValueRef jsValue)
        {
            if (!jsValue.IsValid)
                throw new Exception("Indalid value");
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
                    return new JsObject(jsValue);
                case JsValueType.Function:
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
