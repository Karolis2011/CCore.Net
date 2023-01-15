using System;
using System.Text;

namespace CCore.Net.JsRt
{
    /// <summary>
    ///     A reference to an ES module.
    /// </summary>
    /// <remarks>
    ///     A module record represents an ES module.
    /// </remarks>
    public struct JsModule
    {
        /// <summary>
        /// The reference.
        /// </summary>
        private readonly IntPtr reference;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsModule"/> struct.
        /// </summary>
        /// <param name="reference">The reference.</param>
        private JsModule(IntPtr reference) => this.reference = reference;

        /// <summary>
        ///     Gets an invalid value.
        /// </summary>
        public static JsModule Invalid => new JsModule(IntPtr.Zero);
        
        /// <summary>
        /// Sets callback for receiving notification to fetch a dependent module.
        /// </summary>
        /// <param name="callback">Callback to be invoked.</param>
        public void SetFetchImportedModuleCallback(JsFetchImportedModuleCallBack callback)
        {
            Native.ThrowIfError(Native.JsSetModuleHostInfo(this, JsModuleHostInfoKind.FetchImportedModuleCallback, callback));
        }
        
        /// <summary>
        /// Sets callback for receiving notification when module is ready.
        /// </summary>
        /// <param name="callback">Callback to be invoked.</param>
        public void SetNotifyModuleReadyCallback(JsNotifyModuleReadyCallback callback)
        {
            Native.ThrowIfError(Native.JsSetModuleHostInfo(this, JsModuleHostInfoKind.NotifyModuleReadyCallback, callback));
        }
        
        /// <summary>
        /// Sets abrtitrarily host defined context for the module.
        /// </summary>
        /// <param name="info">Pointer for said data.</param>
        public void SetHostInfo(IntPtr info)
        {
            Native.ThrowIfError(Native.JsSetModuleHostInfo(this, JsModuleHostInfoKind.HostDefined, info));
        }
        
        /// <summary>
        /// Sets URL for the module, to be used in error stack traces and debugging.
        /// </summary>
        /// <param name="url">Url to be set.</param>
        public void SetUrl(string url)
        {
            Native.ThrowIfError(Native.JsSetModuleHostInfo(this, JsModuleHostInfoKind.Url, JsValueRef.From(url)));
        }
        
        /// <summary>
        /// Sets an exception object - e.g. if the module file cannot be found.
        /// </summary>
        /// <param name="error">Js Error object</param>
        public void SetException(JsValueRef error)
        {
            Native.ThrowIfError(Native.JsSetModuleHostInfo(this, JsModuleHostInfoKind.Exception, error));
        }
        
        /// <summary>
        /// Sets callback for receiving notification for calls to ```import()```
        /// </summary>
        /// <param name="callback">Callback to be invoked.</param>
        public void SetFetchImportedModuleFromScriptCallback(JsFetchImportedModuleFromScriptCallBack callback)
        {
            Native.ThrowIfError(Native.JsSetModuleHostInfo(this, JsModuleHostInfoKind.FetchImportedModuleFromScriptCallback, callback));
        }
        
        /// <summary>
        /// Sets callback to allow host to initialize import.meta object properties.
        /// </summary>
        /// <param name="callback">Callback to be invoked.</param>
        public void SetInitializeImportMetaCallback(JsInitializeImportMetaCallback callback)
        {
            Native.ThrowIfError(Native.JsSetModuleHostInfo(this, JsModuleHostInfoKind.InitializeImportMetaCallback, callback));
        }
        
        /// <summary>
        /// Set callback to report module completion or exception thrown when evaluating a module.
        /// </summary>
        /// <param name="callback">Callback to be invoked.</param>
        public void SetReportModuleCompletionCallback(JsReportModuleCompletionCallback callback)
        {
            Native.ThrowIfError(Native.JsSetModuleHostInfo(this, JsModuleHostInfoKind.ReportModuleCompletionCallback, callback));
        }
        
        /// <summary>
        /// Get URL set for this module.
        /// </summary>
        /// <returns>Url that was set.</returns>
        public string GetUrl()
        {
            Native.ThrowIfError(Native.JsGetModuleHostInfo(this, JsModuleHostInfoKind.Url, out JsValueRef url));
            return url.ToString();
        }
        
        /// <summary>
        /// Get an exception object set for this module.
        /// </summary>
        /// <returns>Error object</returns>
        public JsValueRef GetException()
        {
            Native.ThrowIfError(Native.JsGetModuleHostInfo(this, JsModuleHostInfoKind.Exception, out JsValueRef error));
            return error;
        }
        
        /// <summary>
        /// Get host defined context for this module.
        /// </summary>
        /// <returns>Pointer for data</returns>
        public IntPtr GetHostInfo()
        {
            Native.ThrowIfError(Native.JsGetModuleHostInfo(this, JsModuleHostInfoKind.HostDefined, out IntPtr info));
            return info;
        }
        
        public void Parse(string script, JsSourceContext sourceContext)
        {
            var bytes = Encoding.UTF8.GetBytes(script);
            var res = Native.JsParseModuleSource(this, sourceContext, bytes, (uint)bytes.LongLength, JsParseModuleSourceFlags.DataIsUTF8, out var error);
            Native.ThrowIfError(res);
        }

        public JsValueRef Evaluate()
        {
            Native.ThrowIfError(Native.JsModuleEvaluation(this, out var result));
            return result;
        }
        
        public static JsModule NewModule(string specifier)
        {
            Native.ThrowIfError(Native.JsInitializeModuleRecord(JsModule.Invalid, JsValueRef.From(specifier), out var module));
            return module;
        }
        
        private static JsErrorCode SimpleJsNotifyModuleReadyCallback(JsModule readyModule, JsValueRef exceptionVar)
        {
            readyModule.Evaluate();
            return JsErrorCode.NoError;
        }
        
        public static JsModule NewModule(string specifier, string source)
        {
            var module = NewModule(specifier);
            module.SetNotifyModuleReadyCallback(SimpleJsNotifyModuleReadyCallback);
            module.Parse(source, JsSourceContext.None);
            return module;
        }
    }
}