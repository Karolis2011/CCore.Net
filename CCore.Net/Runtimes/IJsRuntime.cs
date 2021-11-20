using CCore.Net;
using CCore.Netged;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.Netimes
{
    public interface IJsRuntime
    {
        JsRuntime InternalRuntime { get; }
        JsContext InternalContext { get; }
        void EnsureReady();
        void Track(JsValue value);
    }
}
