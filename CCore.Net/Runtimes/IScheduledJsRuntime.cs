using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.Net.Runtimes
{
    public interface IScheduledJsRuntime : IJsRuntime
    {
        JsTask<TResult> Do<TResult>(Func<TResult> func, JsTaskPriority priority = JsTaskPriority.LOWEST);
        JsTask Do(Action action, JsTaskPriority priority = JsTaskPriority.LOWEST);
    }
}
