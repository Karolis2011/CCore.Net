using CCore.Net.JsRt;
using CCore.Net.Runtimes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCore.Net.Test.Runtimes.Fixtures
{
    public class BasicJsRuntimeFixture : IDisposable
    {
        public BasicJsRuntime Runtime { get; set; }

        public BasicJsRuntimeFixture()
        {
            Runtime = new BasicJsRuntime(JsRuntimeAttributes.AllowScriptInterrupt);
        }

        public void Dispose()
        {
            Runtime.Dispose();
            Runtime = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
