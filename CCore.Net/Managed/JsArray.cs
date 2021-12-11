using CCore.Net.JsRt;
using System;

namespace CCore.Net.Managed
{
    public class JsArray : JsObject
    {
        public static new bool isSupported(JsValueType type, JsValueRef value) => type == JsValueType.Array;

        public JsArray(uint length)
        {
            jsValueRef = JsValueRef.CreateArray(length);
        }

        public JsArray(JsValueRef jsValue)
        {
            if (!jsValue.IsValid)
                throw new Exception("Invalid value");
            if (!isSupported(jsValue.ValueType, jsValue))
                throw new Exception("Unsupported type");
            jsValueRef = jsValue;
        }
    }
}
