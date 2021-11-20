using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.Net.JsRt
{
    /// <summary>
    ///     Stepping types.
    /// </summary>
    public enum JsDiagStepType
    {
        /// <summary>
        ///     Perform a step operation to next statement.
        /// </summary>
        StepIn = 0,
        /// <summary>
        ///     Perform a step out from the current function.
        /// </summary>
        StepOut,
        /// <summary>
        ///     Perform a single step over after a debug break if the next statement is a function call, else behaves as a stepin.
        /// </summary>
        StepOver,
        /// <summary>
        ///     Perform a single step back to the previous statement (only applicable in TTD mode).
        /// </summary>
        StepBack,
        /// <summary>
        ///     Perform a reverse continue operation (only applicable in TTD mode).
        /// </summary>
        ReverseContinue,
        /// <summary>
        ///     Perform a forward continue operation. Clears any existing step value.
        /// </summary>
        Continue
    };
}
