﻿using System;

namespace CCore.Net.JsRt
{
    /// <summary>
    ///     An exception returned from the Chakra engine.
    /// </summary>
    public class JsException : Exception
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsException"/> class. 
        /// </summary>
        /// <param name="code">The error code returned.</param>
        public JsException(JsErrorCode code) :
            this(code, "A fatal exception has occurred in a JavaScript runtime")
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsException"/> class. 
        /// </summary>
        /// <param name="code">The error code returned.</param>
        /// <param name="message">The error message.</param>
        public JsException(JsErrorCode code, string message) :
            base(message)
        {
            ErrorCode = code;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsException"/> class. 
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected JsException(string message, Exception innerException) :
            base(message, innerException)
        {
            if (message != null)
            {
                ErrorCode = (JsErrorCode)HResult;
            }
        }

        /*
        /// <summary>
        ///     Serializes the exception information.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The streaming context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("code", (uint)code);
        }
        */
        /// <summary>
        ///     Gets the error code.
        /// </summary>
        public JsErrorCode ErrorCode { get; }
    }
}