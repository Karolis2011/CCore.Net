using System;

namespace CCore.Net.JsRt
{
    /// <summary>
    ///     A callback called before collection.
    /// </summary>
    /// <remarks>
    ///     Use <c>JsSetBeforeCollectCallback</c> to register this callback.
    /// </remarks>
    /// <param name="callbackState">The state passed to SetBeforeCollectCallback.</param>
    public delegate void JsBeforeCollectCallback(IntPtr callbackState);
}