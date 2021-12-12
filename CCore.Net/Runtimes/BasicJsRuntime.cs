using CCore.Net.JsRt;
using CCore.Net.Managed;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CCore.Net.Runtimes
{
    public class BasicJsRuntime : IDisposable, IJsRuntime
    {
        protected bool disposedValue;

        protected JsRuntime runtime;
        protected JsContext context;
        protected JsPromiseContinuationCallback jsPromiseContinuationCallback;


        public JsRuntime InternalRuntime => runtime;
        public JsContext InternalContext => context;

        public bool Enabled
        {
            get => !runtime.Disabled;
            set => runtime.Disabled = !value;
        }

        protected LinkedList<WeakReference<JsValue>> managedValues = new LinkedList<WeakReference<JsValue>>();
        protected ConditionalWeakTable<object, WeakReference<JsValue>> managedObjects = new ConditionalWeakTable<object, WeakReference<JsValue>>();

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

        internal virtual void Track(JsValue value)
        {
            managedValues.AddLast(new WeakReference<JsValue>(value));
        }

        internal virtual void TrackManaged(JsValue value, object obj)
        {
            managedObjects.Add(obj, new WeakReference<JsValue>(value));
        }

        public void SetPromiseContinuationCallback(JsPromiseContinuationCallback callback, IntPtr state)
        {
            if (JsContext.Current != context)
                throw new InvalidOperationException("Not in this runtimes context.");
            jsPromiseContinuationCallback = callback;
            JsContext.SetPromiseContinuationCallback(jsPromiseContinuationCallback, state);
        }

        internal bool TryGetExistingManaged(object obj, out JsValue value)
        {
            if(managedObjects.TryGetValue(obj, out var reference))
                if (reference.TryGetTarget(out value))
                {
                    if (value is IJsFreeable freeable && freeable.IsFreeed)
                        return false; // JsValueRef has been finalized, so this JsValue is invalid
                    return true;
                }
            value = null;
            return false;
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
                    foreach (var r in managedValues)
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

        public virtual void SetException(JsValueRef error)
        {
            JsContext.SetException(error);
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
