using System;
using System.Runtime.InteropServices;

namespace CCore.Net.JsRt
{
    /// <summary>
    ///     A JavaScript value.
    /// </summary>
    /// <remarks>
    ///     A JavaScript value is one of the following types of values: Undefined, Null, Boolean, 
    ///     String, Number, or Object.
    /// </remarks>
    public struct JsRef
    {
        /// <summary>
        /// The reference.
        /// </summary>
        private readonly IntPtr reference;

        private JsRef(IntPtr reference) => this.reference = reference;

        public static JsRef Invalid => new JsRef(IntPtr.Zero);

        /// <summary>
        ///     Gets a value indicating whether the refrence is valid.
        /// </summary>
        public bool IsValid
        {
            get { return reference != IntPtr.Zero; }
        }

        public override bool Equals(object obj) => reference.Equals(obj);

        public override int GetHashCode() => reference.GetHashCode();

        public static bool operator ==(JsRef a, JsRef b) => a.reference == b.reference;
        public static bool operator !=(JsRef a, JsRef b) => a.reference != b.reference;

    }
}