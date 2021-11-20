using ChakraCore.Net.JsRt;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.Net.Managed
{
    public class JsBool : JsValue
    {
        public static new bool isSupported(JsValueType type, JsValueRef value) => type == JsValueType.Boolean;

        public static JsBool True => new JsBool(JsValueRef.True);
        public static JsBool False => new JsBool(JsValueRef.False);

        public JsBool(bool value) : this(JsValueRef.From(value))
        {
        }

        public JsBool(JsValueRef jsValue)
        {
            if (!jsValue.IsValid)
                throw new Exception("Invalid value");
            if (!isSupported(jsValue.ValueType, jsValue))
                throw new Exception("Unsupported type.");

            jsValueRef = jsValue;
        }

        public static implicit operator bool(JsBool jsBool) => jsBool.jsValueRef.ToBoolean();
        public static explicit operator JsBool(bool value) => new JsBool(value);

        public static explicit operator JsBool(JsValueRef jsRef)
        {
            var value = (JsValue)jsRef;
            if (value is JsBool jsBool)
                return jsBool;
            else
                return value.ConvertToBoolean();
        }

    }
}
