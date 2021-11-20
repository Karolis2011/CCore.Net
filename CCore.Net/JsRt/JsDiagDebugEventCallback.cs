using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.Net.JsRt
{
    /// <summary>
    ///     User implemented callback routine for debug events.
    /// </summary>
    /// <remarks>
    ///     Use <c>JsDiagStartDebugging</c> to register the callback.
    /// </remarks>
    /// <param name="debugEvent">The type of JsDiagDebugEvent event.</param>
    /// <param name="eventData">Additional data related to the debug event.</param>
    /// <param name="callbackState">The state passed to <c>JsDiagStartDebugging</c>.</param>
    public delegate void JsDiagDebugEventCallback(JsDiagDebugEvent debugEvent, JsValueRef eventData, IntPtr callbackState);
}
