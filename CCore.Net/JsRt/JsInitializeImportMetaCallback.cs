using System;

namespace CCore.Net.JsRt
{
    /// <summary>
    ///     User implemented callback to fill in module properties for the import.meta object.
    /// </summary>
    /// <remarks>
    ///     This callback allows the host to fill module details for the referencing module in the import.meta object
    ///     loaded by script.
    ///     The callback is invoked on the current runtime execution thread, therefore execution is blocked until the
    ///     callback completes.
    /// </remarks>
    /// <param name="referencingModule">The referencing module that is loading an import.meta object.</param>
    /// <param name="importMetaVar">The object which will be returned to script for the referencing module.</param>
    /// <returns>
    ///     Returns a JsErrorCode - note, the return value is ignored.
    /// </returns>
    public delegate JsErrorCode JsInitializeImportMetaCallback(JsModule referencingModule, JsValueRef importMetaVar);
}