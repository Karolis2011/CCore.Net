using CCore.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.Netged
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
                throw new Exception("Indalid value");
            if (!isSupported(jsValue.ValueType, jsValue))
                throw new Exception("Unsupported type");
            jsValueRef = jsValue;
        }
    }
}
