using System;

namespace ChakraCore.Net.JsRt
{
    /// <summary>
    ///     A cookie that identifies a script for debugging purposes.
    /// </summary>
    public struct JsSourceContext : IEquatable<JsSourceContext>
    {
        /// <summary>
        /// The context.
        /// </summary>
        private readonly IntPtr context;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsSourceContext"/> struct.
        /// </summary>
        /// <param name="context">The context.</param>
        private JsSourceContext(IntPtr context)
        {
            this.context = context;
        }

        /// <summary>
        ///     Gets an empty source context.
        /// </summary>
        public static JsSourceContext None
        {
            get { return new JsSourceContext(new IntPtr(-1)); }
        }

        /// <summary>
        ///     The equality operator for source contexts.
        /// </summary>
        /// <param name="left">The first source context to compare.</param>
        /// <param name="right">The second source context to compare.</param>
        /// <returns>Whether the two source contexts are the same.</returns>
        public static bool operator ==(JsSourceContext left, JsSourceContext right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     The inequality operator for source contexts.
        /// </summary>
        /// <param name="left">The first source context to compare.</param>
        /// <param name="right">The second source context to compare.</param>
        /// <returns>Whether the two source contexts are not the same.</returns>
        public static bool operator !=(JsSourceContext left, JsSourceContext right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///     Subtracts an offset from the value of the source context.
        /// </summary>
        /// <param name="context">The source context to subtract the offset from.</param>
        /// <param name="offset">The offset to subtract.</param>
        /// <returns>A new source context that reflects the subtraction of the offset from the context.</returns>
        public static JsSourceContext operator -(JsSourceContext context, int offset)
        {
            return FromIntPtr(context.context - offset);
        }

        /// <summary>
        ///     Subtracts an offset from the value of the source context.
        /// </summary>
        /// <param name="left">The source context to subtract the offset from.</param>
        /// <param name="right">The offset to subtract.</param>
        /// <returns>A new source context that reflects the subtraction of the offset from the context.</returns>
        public static JsSourceContext Subtract(JsSourceContext left, int right)
        {
            return left - right;
        }

        /// <summary>
        ///     Decrements the value of the source context.
        /// </summary>
        /// <param name="context">The source context to decrement.</param>
        /// <returns>A new source context that reflects the decrementing of the context.</returns>
        public static JsSourceContext operator --(JsSourceContext context)
        {
            return FromIntPtr(context.context - 1);
        }

        /// <summary>
        ///     Decrements the value of the source context.
        /// </summary>
        /// <param name="left">The source context to decrement.</param>
        /// <returns>A new source context that reflects the decrementing of the context.</returns>
        public static JsSourceContext Decrement(JsSourceContext left)
        {
            return --left;
        }

        /// <summary>
        ///     Adds an offset from the value of the source context.
        /// </summary>
        /// <param name="context">The source context to add the offset to.</param>
        /// <param name="offset">The offset to add.</param>
        /// <returns>A new source context that reflects the addition of the offset to the context.</returns>
        public static JsSourceContext operator +(JsSourceContext context, int offset)
        {
            return FromIntPtr(context.context + offset);
        }

        /// <summary>
        ///     Adds an offset from the value of the source context.
        /// </summary>
        /// <param name="left">The source context to add the offset to.</param>
        /// <param name="right">The offset to add.</param>
        /// <returns>A new source context that reflects the addition of the offset to the context.</returns>
        public static JsSourceContext Add(JsSourceContext left, int right)
        {
            return left + right;
        }

        /// <summary>
        ///     Increments the value of the source context.
        /// </summary>
        /// <param name="context">The source context to increment.</param>
        /// <returns>A new source context that reflects the incrementing of the context.</returns>
        public static JsSourceContext operator ++(JsSourceContext context)
        {
            return FromIntPtr(context.context + 1);
        }

        /// <summary>
        ///     Increments the value of the source context.
        /// </summary>
        /// <param name="left">The source context to increment.</param>
        /// <returns>A new source context that reflects the incrementing of the context.</returns>
        public static JsSourceContext Increment(JsSourceContext left)
        {
            return ++left;
        }

        /// <summary>
        ///     Creates a new source context. 
        /// </summary>
        /// <param name="cookie">
        ///     The cookie for the source context.
        /// </param>
        /// <returns>The new source context.</returns>
        public static JsSourceContext FromIntPtr(IntPtr cookie)
        {
            return new JsSourceContext(cookie);
        }

        /// <summary>
        ///     Checks for equality between source contexts.
        /// </summary>
        /// <param name="other">The other source context to compare.</param>
        /// <returns>Whether the two source contexts are the same.</returns>
        public bool Equals(JsSourceContext other)
        {
            return context == other.context;
        }

        /// <summary>
        ///     Checks for equality between source contexts.
        /// </summary>
        /// <param name="obj">The other source context to compare.</param>
        /// <returns>Whether the two source contexts are the same.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            return obj is JsSourceContext && Equals((JsSourceContext)obj);
        }

        /// <summary>
        ///     The hash code.
        /// </summary>
        /// <returns>The hash code of the source context.</returns>
        public override int GetHashCode()
        {
            return context.ToInt32();
        }
    }
}