﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.Net.JsRt
{
    /// <summary>
    ///     Break on Exception attributes.
    /// </summary>
    public enum JsDiagBreakOnExceptionAttributes
    {
        /// <summary>
        ///     Don't break on any exception.
        /// </summary>
        None = 0x0,
        /// <summary>
        ///     Break on uncaught exception.
        /// </summary>
        Uncaught = 0x1,
        /// <summary>
        ///     Break on first chance exception.
        /// </summary>
        FirstChance = 0x2
    };
}
