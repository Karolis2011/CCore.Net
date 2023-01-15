using System;

namespace CCore.Net.JsRt
{
    /// <summary>
    ///     User implemented callback to get notification when the module is ready.
    /// </summary>
    /// <remarks>
    ///     The callback is invoked on the current runtime execution thread, therefore execution is blocked until the
    ///     callback completes. This callback should schedule a call to JsEvaluateModule to run the module that has been loaded.
    /// </remarks>
    /// <param name="referencingModule">The referencing module that has finished running ModuleDeclarationInstantiation step.</param>
    /// <param name="exceptionVar">If nullptr, the module is successfully initialized and host should queue the execution job
    ///                            otherwise it's the exception object.</param>
    /// <returns>
    ///     Returns a JsErrorCode - note, the return value is ignored.
    /// </returns>
    public delegate JsErrorCode JsNotifyModuleReadyCallback(JsModule referencingModule, JsValueRef exceptionVar);
}