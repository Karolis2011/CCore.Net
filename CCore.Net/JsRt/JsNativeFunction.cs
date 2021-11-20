using System;
using System.Runtime.InteropServices;

namespace CCore.Net.JsRt
{
    /// <summary>
    ///     A function callback.
    /// </summary>
    /// <param name="callee">
    ///     A <c>Function</c> object that represents the function being invoked.
    /// </param>
    /// <param name="isConstructCall">Indicates whether this is a regular call or a 'new' call.</param>
    /// <param name="arguments">The arguments to the call.</param>
    /// <param name="argumentCount">The number of arguments.</param>
    /// <param name="callbackState">The state passed to <c>JsCreateFunction</c>.</param>
    /// <returns>The result of the call, if any.</returns>
    public delegate JsValueRef JsNativeFunction(
        JsValueRef callee,
        [MarshalAs(UnmanagedType.U1)] bool isConstructCall,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] JsValueRef[] arguments,
        ushort argumentCount,
        IntPtr callbackState);

}