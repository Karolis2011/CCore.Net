using CCore.Net.JsRt;
using System;

namespace CCore.Net.Managed
{
    public class JsString : JsValue
    {
        public static new bool isSupported(JsValueType type, JsValueRef value) => type == JsValueType.String;


        public JsString(string value)
        {
            jsValueRef = JsValueRef.From(value);
        }


        public JsString(JsValueRef jsValue)
        {
            if (!jsValue.IsValid)
                throw new Exception("Invalid value");
            if (!isSupported(jsValue.ValueType, jsValue))
                throw new Exception("Unsupported type.");

            jsValueRef = jsValue;
        }

        public override string ToString() => jsValueRef.ToString();

        public static implicit operator string(JsString value) => value.jsValueRef.ToString();

        public static implicit operator JsString(string value) => new JsString(value);
    }
}
