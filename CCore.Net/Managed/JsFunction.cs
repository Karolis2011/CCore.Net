using CCore.Net.JsRt;
using System;
using System.Linq;

namespace CCore.Net.Managed
{
    public class JsFunction : JsObject
    {
        public static new bool isSupported(JsValueType type, JsValueRef value) => type == JsValueType.Function;

        protected JsFunction() : base() { }

        public JsFunction(JsValueRef jsValue)
        {
            if (!jsValue.IsValid)
                throw new Exception("Indalid value");
            if (!isSupported(jsValue.ValueType, jsValue))
                throw new Exception("Invalid Value type");
            jsValueRef = jsValue;
        }

        /// <summary>
        /// Invokes a function.
        /// </summary>
        /// <param name="arguments">The arguments to the call. First argument is this. Same as in js function `call()`</param>
        /// <returns>The <c>Value</c> returned from the function invocation, if any.</returns>
        internal JsValueRef Invoke(params JsValueRef[] arguments) => jsValueRef.CallFunction(arguments);

        /// <summary>
        /// Invokes a function.
        /// </summary>
        /// <param name="arguments">The arguments to the call. First argument is this. Same as in js function `call()`</param>
        /// <returns>The <c>Value</c> returned from the function invocation, if any.</returns>
        internal JsValue Invoke(params JsValue[] arguments) => (JsValue)Invoke(arguments: arguments.Select(v => v.jsValueRef).ToArray());


        /// <summary>
        /// Invokes a function.
        /// </summary>
        /// <param name="thisArg">This argument to be used in function call</param>
        /// <param name="arguments">The arguments to the call.</param>
        /// <returns>The <c>Value</c> returned from the function invocation, if any.</returns>
        public JsValueRef Invoke(JsValueRef thisArg, params JsValueRef[] arguments) => jsValueRef.CallFunction(thisArg, arguments);

        /// <summary>
        /// Invokes a function.
        /// </summary>
        /// <param name="thisArg">This argument to be used in function call</param>
        /// <param name="arguments">The arguments to the call.</param>
        /// <returns>The <c>Value</c> returned from the function invocation, if any.</returns>
        public JsValue Invoke(JsValue thisArg, params JsValue[] arguments) => (JsValue)Invoke(thisArg: thisArg.jsValueRef, arguments: arguments.Select(v => v.jsValueRef).ToArray());
    }
}
