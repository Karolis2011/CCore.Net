using CCore.Net.JsRt;
using CCore.Net.Runtimes;
using System;

namespace CCore.Net.Managed
{
    public class JsValue : IDisposable
    {
        internal JsValueRef jsValueRef;
        protected bool refed;
        internal BasicJsRuntime runtime;

        private bool disposedValue;

        public static bool isSupported(JsValueType type, JsValueRef value) => value.IsValid;

        public static JsValue FromRaw(JsValueRef jsValue)
        {
            if (!jsValue.IsValid)
                throw new Exception("Indalid value");
            var type = jsValue.ValueType;
            switch (type)
            {
                case JsValueType.Undefined:
                    return new JsUndefined(jsValue);
                case JsValueType.Null:
                    return new JsNull(jsValue);
                case JsValueType.Number:
                    return new JsNumber(jsValue);
                case JsValueType.String:
                    return new JsString(jsValue);
                case JsValueType.Boolean:
                    return new JsBool(jsValue);
                case JsValueType.Object:
                    return new JsObject(jsValue);
                case JsValueType.Function:
                    return new JsFunction(jsValue);
                case JsValueType.Error:
                    return new JsObject(jsValue);
                case JsValueType.Array:
                    return new JsArray(jsValue);
                case JsValueType.Symbol:
                case JsValueType.ArrayBuffer:
                case JsValueType.TypedArray:
                case JsValueType.DataView:
                    return new JsObject(jsValue); // They are close enougth to objects
                default:
                    throw new ArgumentOutOfRangeException();
            }
            throw new NotImplementedException("Unsupported value");
        }



        protected JsValue()
        {
            runtime = BasicJsRuntime.ActiveRuntime;
            if (runtime == null)
                throw new Exception("Not in managed runtime.");
            runtime.Track(this);
        }

        public JsValue(JsValueRef jsValue)
        {
            if (!jsValue.IsValid)
                throw new Exception("Invalid value");
            jsValueRef = jsValue;

            runtime = BasicJsRuntime.ActiveRuntime;
            if (runtime == null)
                throw new Exception("Not in managed runtime.");
        }

        public void AddRef()
        {
            if (!refed)
            {
                refed = true;
                if (runtime == BasicJsRuntime.ActiveRuntime)
                    jsValueRef.AddRef();
                else
                {
                    if (runtime is ScheduledJsRuntime jsRuntime)
                        jsRuntime.Do(() => jsValueRef.AddRef(), JsTaskPriority.INITIALIZATION);
                    else
                    {
                        refed = false;
                        throw new Exception("Can't ref object on basic js rumtine without call being in runtime context.");
                    }
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }
                if (refed)
                {
                    if (runtime == BasicJsRuntime.ActiveRuntime)
                        jsValueRef.Release();
                    else
                    {
                        if (runtime is IScheduledJsRuntime jsRuntime)
                            jsRuntime.Do(() => jsValueRef.Release());
                        else
                            throw new Exception("Can't unref refed object outside it's runtime.");
                    }
                }
                // set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~JsValue()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public static implicit operator JsValueRef(JsValue value) => value.jsValueRef;
        public static explicit operator JsValue(JsValueRef v) => FromRaw(v);


        public JsString ConvertToString() => new JsString(jsValueRef.ConvertToString());
        public JsBool ConvertToBoolean() => new JsBool(jsValueRef.ConvertToBoolean());
        public JsNumber ConvertToNumber() => new JsNumber(jsValueRef.ConvertToNumber());
        public JsObject ConvertToObject() => new JsObject(jsValueRef.ConvertToObject());

    }
}
