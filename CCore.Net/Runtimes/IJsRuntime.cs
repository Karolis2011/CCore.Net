using CCore.Net.JsRt;
using CCore.Net.Managed;

namespace CCore.Net.Runtimes
{
    public interface IJsRuntime
    {
        JsRuntime InternalRuntime { get; }
        JsContext InternalContext { get; }
        void EnsureReady();
    }
}
