using CCore.Net.JsRt;
using CCore.Net.Runtimes;
using System;

namespace CCore.Net.Managed
{
    public class JsValue : IDisposable
    {
        internal const string ExternalObjectPropertyName = "_cctm_externalObject";

        internal JsValueRef jsValueRef;
        protected bool refed;
        internal BasicJsRuntime runtime;

        private bool disposedValue;

        public static bool IsSupported(JsValueType type, JsValueRef value) => value.IsValid;

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

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if(obj is JsValue jsValue)
            {
                return jsValueRef.Equals(jsValue.jsValueRef);
            }
            return false;
        }

        public bool StrictEquals(JsValue obj)
        {
            return jsValueRef.StrictEquals(obj.jsValueRef);
        } 

        public override int GetHashCode() => jsValueRef.GetHashCode();

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
        public static explicit operator JsValue(JsValueRef v) => JsTypeMapper.FromRaw(v);


        public JsString ConvertToString() => new JsString(jsValueRef.ConvertToString());
        public JsBool ConvertToBoolean() => new JsBool(jsValueRef.ConvertToBoolean());
        public JsNumber ConvertToNumber() => new JsNumber(jsValueRef.ConvertToNumber());
        public JsObject ConvertToObject() => new JsObject(jsValueRef.ConvertToObject());

    }
}
