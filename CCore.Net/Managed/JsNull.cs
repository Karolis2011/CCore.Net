using CCore.Net.JsRt;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.Net.Managed
{
    public class JsNull : JsValue
    {
        public static new bool isSupported(JsValueType type, JsValueRef value) => type == JsValueType.Null;

        public static JsNull Null => new JsNull(JsValueRef.Null);

        public JsNull(JsValueRef jsValue)
        {
            if (!jsValue.IsValid)
                throw new Exception("Invalid value");
            if (!isSupported(jsValue.ValueType, jsValue))
                throw new Exception("Unsupported type.");

            jsValueRef = jsValue;
        }
    }
}
