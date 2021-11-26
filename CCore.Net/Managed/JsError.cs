using CCore.Net.JsRt;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.Net.Managed
{
    public class JsError : JsObject
    {
        public static new bool isSupported(JsValueType type, JsValueRef value) => type == JsValueType.Error;

        public JsError(JsValueRef jsValue)
        {
            if (!jsValue.IsValid)
                throw new Exception("Invalid value");
            if (!isSupported(jsValue.ValueType, jsValue))
                throw new Exception("Unsupported type.");

            jsValueRef = jsValue;
        }

        public JsError(string message)
        {
            jsValueRef = JsValueRef.CreateError(message);
        }
    }
}
