using CCore.Net.JsRt;
using System;

namespace CCore.Net.Managed
{
    public class JsObject : JsValue
    {
        public static new bool isSupported(JsValueType type, JsValueRef value) => type == JsValueType.Object;

        public static JsObject GlobalObject => new JsObject(JsValueRef.GlobalObject);

        public static JsObject NewObject() => new JsObject(JsValueRef.CreateObject());

        protected JsObject() { }

        public JsObject(JsValueRef jsValue)
        {
            if (!jsValue.IsValid)
                throw new Exception("Indalid value");
            if (!isSupported(jsValue.ValueType, jsValue))
                throw new Exception("Unsupported type");
            jsValueRef = jsValue;
        }

        public JsValueRef this[JsValueRef index]
        {
            get => jsValueRef.GetIndexedProperty(index);
            set => jsValueRef.SetIndexedProperty(index, value);
        }

        public JsValueRef this[int index]
        {
            get => this[JsValueRef.From(index)];
            set => this[JsValueRef.From(index)] = value;
        }

        public JsValueRef this[string index]
        {
            get => this[JsValueRef.From(index)];
            set => this[JsValueRef.From(index)] = value;
        }

        internal void SetNonEnumerableProperty(string name, JsValueRef value)
        {
            JsObject descriptorValue = NewObject();
            descriptorValue["enumerable"] = JsValueRef.False;
            descriptorValue["writable"] = JsValueRef.True;

            var key = (JsString)name;
            jsValueRef.ObjectDefineProperty(key, descriptorValue);
            this[(JsValueRef)key] = value;
        }
    }
}
