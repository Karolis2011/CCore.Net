using CCore.Net.JsRt;
using System;
using System.Linq;

namespace CCore.Net.Managed
{
    public class JsFunction : JsObject
    {
        public static new bool isSupported(JsValueType type, JsValueRef value) => type == JsValueType.Function;

        public JsFunction(JsValueRef jsValue)
        {
            if (!jsValue.IsValid)
                throw new Exception("Indalid value");
            if (!isSupported(jsValue.ValueType, jsValue))
                throw new Exception("Invalid Value type");
            jsValueRef = jsValue;
        }

        public JsValueRef Invoke(params JsValueRef[] arguments) => jsValueRef.CallFunction(arguments);

        public JsValue Invoke(params JsValue[] arguments) => (JsValue)Invoke(arguments: arguments.Select(v => (jsValueRef)).ToArray());
    }
}
