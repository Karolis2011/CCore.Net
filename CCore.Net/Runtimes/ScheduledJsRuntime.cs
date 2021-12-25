using CCore.Net;
using CCore.Net.Managed;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.Net.Runtimes
{
    public class ScheduledJsRuntime : ScheduledJsRuntime<JsPFIFOScheduler>
    {
        public ScheduledJsRuntime() : base(new JsPFIFOScheduler()) { }
        public ScheduledJsRuntime(JsRuntimeAttributes runtimeAttributes) : base(runtimeAttributes, new JsPFIFOScheduler()) { }
    }

    public class ScheduledJsRuntime<TScheduler> : BasicJsRuntime, IScheduledJsRuntime where TScheduler : JsScheduler
    {
        protected JsScheduler scheduler;

        private JsTask initilizationTask;
        private bool ready = false;

        public TScheduler Scheduler => (TScheduler)scheduler;

        public ScheduledJsRuntime(TScheduler scheduler) : this(JsRuntimeAttributes.None, scheduler)
        {
        }

        public ScheduledJsRuntime(JsRuntimeAttributes runtimeAttributes, TScheduler scheduler) : base(runtimeAttributes)
        {
            this.scheduler = scheduler;
            initilizationTask = this.scheduler.Run(() =>
            {
                ready = true;
            }, JsTaskPriority.INITIALIZATION);
        }

        public override void EnsureReady()
        {
            if (!ready)
                initilizationTask.Wait();
        }

        public virtual JsTask<TResult> Do<TResult>(Func<TResult> func, JsTaskPriority priority = JsTaskPriority.LOWEST) => scheduler.Run(() =>
        {
            using (new Scope(this))
            {
                return func();
            }
        }, priority);

        public virtual JsTask Do(Action action, JsTaskPriority priority = JsTaskPriority.LOWEST) => scheduler.Run(() =>
        {
            using (new Scope(this))
            {
                action();
            }
        }, priority);

        public virtual JsTask<TResult> DoTimed<TResult>(Func<TResult> func, Action onTimeout, TimeSpan timeout, JsTaskPriority priority = JsTaskPriority.LOWEST) => scheduler.RunTimed(() =>
        {
            using (new Scope(this))
            {
                return func();
            }
        }, onTimeout, timeout, priority);

        public virtual JsTask DoTimed(Action action, Action onTimeout, TimeSpan timeout, JsTaskPriority priority = JsTaskPriority.LOWEST) => scheduler.RunTimed(() =>
        {
            using (new Scope(this))
            {
                action();
            }
        }, onTimeout, timeout, priority);

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Do(() =>
                {
                    foreach (var r in managedValues)
                    {
                        if (r.TryGetTarget(out JsValue value))
                        {
                            value.Dispose();
                        }
                    }
                }).Wait();
                if (scheduler is IDisposable disposableScheduler)
                {
                    disposableScheduler.Dispose();
                }
                context.Release();
                runtime.Dispose();
                disposedValue = true;
            }
        }
    }
}
