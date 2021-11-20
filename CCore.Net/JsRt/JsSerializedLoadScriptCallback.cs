namespace CCore.Net
{
    /// <summary>
    ///     Called by the runtime to load the source code of the serialized script.
    /// </summary>
    /// <param name="sourceContext">The context passed to Js[Parse|Run]SerializedScriptCallback</param>
    /// <param name="value">The script returned.</param>
    /// <param name="parseAttributes">Parse Attributes returned.</param>
    /// <returns>
    ///     true if the operation succeeded, false otherwise.
    /// </returns>
    public delegate bool JsSerializedLoadScriptCallback(JsSourceContext sourceContext, out JsValueRef value, out JsValueRef parseAttributes);
}