using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CCore.Net
{

    /// <summary>
    /// A timed JsTask witch specified task action to be executed on timeout
    /// </summary>
    public class JsTaskTimed : JsTask
    {
        protected Action timeoutAction;
        protected Stopwatch timer = new Stopwatch();
        protected TimeSpan timeout;

        /// <summary>
        /// Creates a new Timed task
        /// </summary>
        /// <param name="function">Main action to run</param>
        /// <param name="onTimeout">timeout action</param>
        /// <param name="timeout">timeout time</param>
        /// <param name="priority">priority of this task</param>
        public JsTaskTimed(Action function, Action onTimeout, TimeSpan timeout, JsTaskPriority priority = JsTaskPriority.LOWEST) : base(function, priority)
        {
            timeoutAction = onTimeout;
            this.timeout = timeout;
        }

        /// <inheritdoc/>
        public override void Run()
        {
            timer.Start();
            using (var timer = new Timer(TimmerCheck, null, TimeSpan.Zero, new TimeSpan(10_000)))
            {
                base.Run();
            }
            timer.Stop();
        }

        private void TimmerCheck(object state)
        {
            if (timer.Elapsed > timeout)
            {
                timer.Stop();
                timeoutAction();
            }
        }

        public override void OnBreak()
        {
            base.OnBreak();
            timer.Stop();
        }

        public override void OnResume()
        {
            base.OnResume();
            timer.Start();
        }
    }

    /// <summary>
    /// A timed JsTask witch specified task action to be executed on timeout, with return result
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class JsTaskTimed<TResult> : JsTask<TResult>
    {
        protected Action timeoutAction;
        protected Stopwatch timer = new Stopwatch();
        protected TimeSpan timeout;

        /// <summary>
        /// Creates a new Timed task
        /// </summary>
        /// <param name="function">Main function to run</param>
        /// <param name="onTimeout">timeout action</param>
        /// <param name="timeout">timeout time</param>
        /// <param name="priority">priority of this task</param>
        public JsTaskTimed(Func<TResult> function, Action onTimeout, TimeSpan timeout, JsTaskPriority priority = JsTaskPriority.LOWEST) : base(function, priority)
        {
            timeoutAction = onTimeout;
            this.timeout = timeout;
        }

        /// <inheritdoc/>
        public override void Run()
        {
            timer.Start();
            using (var timer = new Timer(TimmerCheck, null, TimeSpan.Zero, new TimeSpan(10_000)))
            {
                base.Run();
            }
            timer.Stop();
        }

        private void TimmerCheck(object state)
        {
            if (timer.Elapsed > timeout)
            {
                timer.Stop();
                timeoutAction();
            }
        }

        public override void OnBreak()
        {
            base.OnBreak();
            timer.Stop();
        }

        public override void OnResume()
        {
            base.OnResume();
            timer.Start();
        }
    }
}
