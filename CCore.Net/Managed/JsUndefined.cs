using CCore.Net.JsRt;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.Net.Managed
{
    public class JsUndefined : JsValue
    {
        public static new bool isSupported(JsValueType type, JsValueRef value) => type == JsValueType.Undefined;

        public static JsUndefined Undefined => new JsUndefined(JsValueRef.Undefined);

        public JsUndefined(JsValueRef jsValue)
        {
            if (!jsValue.IsValid)
                throw new Exception("Invalid value");
            if (!isSupported(jsValue.ValueType, jsValue))
                throw new Exception("Unsupported type.");

            jsValueRef = jsValue;
        }
    }
}
