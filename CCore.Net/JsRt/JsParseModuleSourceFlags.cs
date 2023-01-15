using System;

namespace CCore.Net.JsRt
{
    /// <summary>
    ///     Flags for parsing a module.
    /// </summary>
    [Flags]
    public enum JsParseModuleSourceFlags
    {
        /// <summary>
        ///     Module source is UTF16.
        /// </summary>
        DataIsUTF16LE = 0x0,

        /// <summary>
        ///     Module source is UTF8.
        /// </summary>
        DataIsUTF8 = 0x1

    }
}