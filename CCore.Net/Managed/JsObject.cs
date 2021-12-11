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

        internal void SetInternalProperty(string name, JsValueRef value)
        {

            JsObject descriptorValue = NewObject();
            descriptorValue["configurable"] = JsValueRef.False;
            descriptorValue["enumerable"] = JsValueRef.False;
            descriptorValue["writable"] = JsValueRef.False;
            descriptorValue["value"] = value;

            DefineProperty((JsString)name, descriptorValue);
        }

        public bool DefineProperty(JsValueRef key, JsValueRef descriptor) => jsValueRef.ObjectDefineProperty(key, descriptor);

        /// <summary>
        /// A short hand to `Object.freeze(object)`
        /// </summary>
        public void Freeze() => ((JsFunction)((JsObject)GlobalObject["Object"])["freeze"]).Invoke(GlobalObject, jsValueRef);
    }
}
