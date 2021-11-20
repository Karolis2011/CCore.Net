using System;

namespace CCore.Net.JsRt
{
    /// <summary>
    ///     A fatal exception occurred.
    /// </summary>
    public sealed class JsFatalException : JsException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="JsFatalException"/> class. 
        /// </summary>
        /// <param name="code">The error code returned.</param>
        public JsFatalException(JsErrorCode code) :
            this(code, "A fatal exception has occurred in a JavaScript runtime")
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsFatalException"/> class. 
        /// </summary>
        /// <param name="code">The error code returned.</param>
        /// <param name="message">The error message.</param>
        public JsFatalException(JsErrorCode code, string message) :
            base(code, message)
        {
        }
    }
}