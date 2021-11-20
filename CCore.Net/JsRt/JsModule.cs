using System;
using System.Runtime.InteropServices;

namespace CCore.Net
{
    /// <summary>
    ///     A JavaScript value.
    /// </summary>
    /// <remarks>
    ///     A JavaScript value is one of the following types of values: Undefined, Null, Boolean, 
    ///     String, Number, or Object.
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


    }
}