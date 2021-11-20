using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.Net.JsRt
{
    /// <summary>
    ///     Debug events reported from ChakraCore engine.
    /// </summary>
    public enum JsDiagDebugEvent
    {
        /// <summary>
        ///     Indicates a new script being compiled, this includes script, eval, new function.
        /// </summary>
        SourceCompile = 0,
        /// <summary>
        ///     Indicates compile error for a script.
        /// </summary>
        CompileError,
        /// <summary>
        ///     Indicates a break due to a breakpoint.
        /// </summary>
        Breakpoint,
        /// <summary>
        ///     Indicates a break after completion of step action.
        /// </summary>
        StepComplete,
        /// <summary>
        ///     Indicates a break due to debugger statement.
        /// </summary>
        DebuggerStatement,
        /// <summary>
        ///     Indicates a break due to async break.
        /// </summary>
        AsyncBreak,
        /// <summary>
        ///     Indicates a break due to a runtime script exception.
        /// </summary>
        RuntimeException
    };
}
