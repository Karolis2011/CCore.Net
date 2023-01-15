using System;

namespace CCore.Net.JsRt
{
    /// <summary>
///     User implemented callback to report completion of module execution.
/// </summary>
/// <remarks>
///     This callback is used to report the completion of module execution and to report any runtime exceptions.
///     Note it is not used for dynamicly imported modules import() as the reuslt from those are handled with a
///     promise.
///     If this callback is not set and a module produces an exception:
///     a) a purely synchronous module tree with an exception will set the exception on the runtime
///        (this is not done if this callback is set)
///     b) an exception in an asynchronous module tree will not be reported directly.
///
///     However in all cases the exception will be set on the JsModuleRecord.
/// </remarks>
/// <param name="module">The root module that has completed either with an exception or normally.</param>
/// <param name="exception">The exception object which was thrown or nullptr if the module had a normal completion.</param>
/// <returns>
///     Returns a JsErrorCode: JsNoError if successful.
/// </returns>
    public delegate JsErrorCode JsReportModuleCompletionCallback(JsModule module, JsValueRef exception);
}