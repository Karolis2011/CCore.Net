using CCore.Net;
using CCore.Netged;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.Netimes
{
    public class BasicJsRuntime : IDisposable, IJsRuntime
    {
        protected bool disposedValue;

        protected JsRuntime runtime;
        protected JsContext context;


        public JsRuntime InternalRuntime => runtime;
        public JsContext InternalContext => context;



        protected LinkedList<WeakReference<JsValue>> managedObjects = new LinkedList<WeakReference<JsValue>>();

        public BasicJsRuntime(JsRuntimeAttributes runtimeAttributes)
        {
            runtime = JsRuntime.Create(runtimeAttributes);
            context = runtime.CreateContext();
            context.AddRef();
        }

        public virtual void EnsureReady()
        {
            if (disposedValue)
                throw new ObjectDisposedException(nameof(BasicJsRuntime));
        }

        public virtual void Track(JsValue value)
        {
            managedObjects.AddLast(new WeakReference<JsValue>(value));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }
                using (new Scope(this))
                {
                    foreach (var r in managedObjects)
                    {
                        if (r.TryGetTarget(out JsValue value))
                        {
                            value.Dispose();
                        }
                    }
                }
                context.Release();
                runtime.Dispose();
                disposedValue = true;
            }
        }

        ~BasicJsRuntime()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        [ThreadStatic]
        private static BasicJsRuntime _activeRuntime;

        /// <summary>
        /// Gets or sets currently active BasicJsRuntime.
        /// </summary>
        public static BasicJsRuntime ActiveRuntime
        {
            get => _activeRuntime;
            set
            {
                _activeRuntime = value;
                if (_activeRuntime != null && _activeRuntime.context.IsValid)
                {
                    JsContext.Current = _activeRuntime.context;
                }
                else
                {
                    JsContext.Current = JsContext.Invalid;
                }
            }
        }
        public class Scope : IDisposable
        {
            private BasicJsRuntime prevousRuntime;
            private bool disposedValue;

            public Scope(BasicJsRuntime runtime)
            {
                prevousRuntime = ActiveRuntime;
                runtime.EnsureReady();
                ActiveRuntime = runtime;
            }
            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        ActiveRuntime = prevousRuntime;
                    }
                    disposedValue = true;
                }
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
