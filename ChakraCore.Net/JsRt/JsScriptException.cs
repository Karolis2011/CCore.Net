using System;
using System.Runtime.Serialization;

namespace ChakraCore.Net.JsRt
{
    /// <summary>
    ///     A script exception.
    /// </summary>
    public sealed class JsScriptException : JsException
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsScriptException"/> class. 
        /// </summary>
        /// <param name="code">The error code returned.</param>
        /// <param name="error">The JavaScript error object.</param>
        public JsScriptException(JsErrorCode code, JsValueRef error) :
            this(code, error, "Error")
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsScriptException"/> class. 
        /// </summary>
        /// <param name="code">The error code returned.</param>
        /// <param name="error">The JavaScript error object.</param>
        /// <param name="message">The error message.</param>
        public JsScriptException(JsErrorCode code, JsValueRef error, string message) :
            base(code, message)
        {
            Error = error;
        }

        /// <summary>
        ///     Gets a JavaScript object representing the script error.
        /// </summary>
        public JsValueRef Error { get; }

        public static JsScriptException FromError(JsErrorCode code, JsValueRef error, string errorName)
        {
            var finalMessage = errorName;
            if(error.ValueType == JsValueType.Error)
            {
                var lineInfo = "";
                var extendedMessage = "<Failed to obtain information>";
                //var st = JsValue.JsonStringify();
                //var full = st(error);
                var names = error.GetOwnPropertyNames();
                //var namesStr = st(names);
                var includes = names.GetProperty("includes");
                // {"description":"Expected ';'","message":"Expected ';'","line":5,"column":5,"length":4,"source":"This will error","url":""}
                if (includes.CallFunction(names, JsValueRef.From("line")).ToBoolean())
                {
                    var line = error.GetProperty("line").ToInt32();
                    var col = error.GetProperty("column").ToInt32();
                    lineInfo = $" ({line+1}, {col+1})";
                }
                if (includes.CallFunction(names, JsValueRef.From("stack")).ToBoolean())
                {
                    var a = error.GetProperty("stack");
                    if (a.ValueType == JsValueType.String)
                    {
                        extendedMessage = a.ToString();
                        lineInfo = "";
                    }
                } else
                {
                    var toString = error.GetProperty("toString");
                    extendedMessage = toString.CallFunction(error).ToString();
                }

                finalMessage = $"{errorName}{lineInfo}: {extendedMessage}";
            }
            return new JsScriptException(code, error, finalMessage);
        }
    }
}