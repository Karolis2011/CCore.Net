using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.Net.Runtimes
{
    public interface IScheduledJsRuntime : IJsRuntime
    {
        JsTask<TResult> Do<TResult>(Func<TResult> func, JsTaskPriority priority = JsTaskPriority.LOWEST);
        JsTask Do(Action action, JsTaskPriority priority = JsTaskPriority.LOWEST);

        JsTask<TResult> DoTimed<TResult>(Func<TResult> func, Action onTimeout, TimeSpan timeout, JsTaskPriority priority = JsTaskPriority.LOWEST);
        JsTask DoTimed(Action action, Action onTimeout, TimeSpan timeout, JsTaskPriority priority = JsTaskPriority.LOWEST);
    }
}
