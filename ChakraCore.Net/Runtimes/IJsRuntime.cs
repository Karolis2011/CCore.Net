using ChakraCore.Net.JsRt;
using ChakraCore.Net.Managed;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.Net.Runtimes
{
    public interface IJsRuntime
    {
        JsRuntime InternalRuntime { get; }
        JsContext InternalContext { get; }
        void EnsureReady();
        void Track(JsValue value);
    }
}
