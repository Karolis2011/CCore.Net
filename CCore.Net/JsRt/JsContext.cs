using System;

namespace CCore.Net.JsRt
{
    /// <summary>
    ///     A script context.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     Each script context contains its own global object, distinct from the global object in 
    ///     other script contexts.
    ///     </para>
    ///     <para>
    ///     Many Chakra hosting APIs require an "active" script context, which can be set using 
    ///     Current. Chakra hosting APIs that require a current context to be set will note 
    ///     that explicitly in their documentation.
    ///     </para>
    /// </remarks>
    public struct JsContext
    {
        /// <summary>
        ///     The reference.
        /// </summary>
        private readonly JsRef reference;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsContext"/> struct. 
        /// </summary>
        /// <param name="reference">The reference.</param>
        internal JsContext(JsRef reference)
        {
            this.reference = reference;
        }

        /// <summary>
        ///     Gets an invalid context.
        /// </summary>
        public static JsContext Invalid
        {
            get { return new JsContext(JsRef.Invalid); }
        }

        /// <summary>
        ///     Gets or sets the current script context on the thread.
        /// </summary>
        public static JsContext Current
        {
            get
            {
                Native.ThrowIfError(Native.JsGetCurrentContext(out JsContext reference));
                return reference;
            }

            set
            {
                Native.ThrowIfError(Native.JsSetCurrentContext(value));
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the runtime of the current context is in an exception state.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     If a call into the runtime results in an exception (either as the result of running a 
        ///     script or due to something like a conversion failure), the runtime is placed into an 
        ///     "exception state." All calls into any context created by the runtime (except for the 
        ///     exception APIs) will fail with <c>InExceptionState</c> until the exception is 
        ///     cleared.
        ///     </para>
        ///     <para>
        ///     If the runtime of the current context is in the exception state when a callback returns 
        ///     into the engine, the engine will automatically rethrow the exception.
        ///     </para>
        ///     <para>
        ///     Requires an active script context.
        ///     </para>
        /// </remarks>
        public static bool HasException
        {
            get
            {
                Native.ThrowIfError(Native.JsHasException(out bool hasException));
                return hasException;
            }
        }

        /// <summary>
        ///     Gets the runtime that the context belongs to.
        /// </summary>
        public JsRuntime Runtime
        {
            get
            {
                Native.ThrowIfError(Native.JsGetRuntime(this, out JsRuntime handle));
                return handle;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the context is a valid context or not.
        /// </summary>
        public bool IsValid => reference.IsValid;

        /// <summary>
        ///     Tells the runtime to do any idle processing it need to do.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     If idle processing has been enabled for the current runtime, calling <c>Idle</c> will 
        ///     inform the current runtime that the host is idle and that the runtime can perform 
        ///     memory cleanup tasks.
        ///     </para>
        ///     <para>
        ///     <c>Idle</c> will also return the number of system ticks until there will be more idle work
        ///     for the runtime to do. Calling <c>Idle</c> before this number of ticks has passed will do
        ///     no work.
        ///     </para>
        ///     <para>
        ///     Requires an active script context.
        ///     </para>
        /// </remarks>
        /// <returns>
        ///     The next system tick when there will be more idle work to do. Returns the 
        ///     maximum number of ticks if there no upcoming idle work to do.
        /// </returns>
        public static uint Idle()
        {
            Native.ThrowIfError(Native.JsIdle(out uint ticks));
            return ticks;
        }

        /// <summary>
        ///     Parses a script and returns a <c>Function</c> representing the script.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="script">The script to parse.</param>
        /// <param name="sourceContext">
        ///     A cookie identifying the script that can be used by script contexts that have debugging enabled.
        /// </param>
        /// <param name="sourceName">The location the script came from.</param>
        /// <returns>A <c>Function</c> representing the script code.</returns>
        public static JsValueRef ParseScript(string script, JsSourceContext sourceContext, string sourceName)
        {
            Native.ThrowIfError(Native.JsParse(JsValueRef.From(script), sourceContext, JsValueRef.From(sourceName), JsParseScriptAttributes.None, out JsValueRef result));
            return result;
        }

        /// <summary>
        ///     Parses a script and returns a <c>Function</c> representing the script.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="script">The script to parse.</param>
        /// <returns>A <c>Function</c> representing the script code.</returns>
        public static JsValueRef ParseScript(string script)
        {
            return ParseScript(script, JsSourceContext.None, string.Empty);
        }

        /// <summary>
        ///     Executes a script.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="script">The script to run.</param>
        /// <param name="sourceContext">
        ///     A cookie identifying the script that can be used by script contexts that have debugging enabled.
        /// </param>
        /// <param name="sourceName">The location the script came from.</param>
        /// <returns>The result of the script, if any.</returns>
        public static JsValueRef RunScript(string script, JsSourceContext sourceContext, string sourceName)
        {
            return Run(JsValueRef.From(script), sourceContext, JsValueRef.From(sourceName), JsParseScriptAttributes.None);
        }

        /// <summary>
        ///     Executes a script.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="script">The script to run.</param>
        /// <returns>The result of the script, if any.</returns>
        public static JsValueRef RunScript(string script)
        {
            return RunScript(script, JsSourceContext.None, string.Empty);
        }

        /// <summary>
        ///     Executes a script.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="script">The script to run.</param>
        /// <param name="sourceContext">
        ///     A cookie identifying the script that can be used by script contexts that have debugging enabled.
        /// </param>
        /// <param name="sourceName">The location the script came from.</param>
        /// <returns>The result of the script, if any.</returns>
        public static JsValueRef Run(JsValueRef script, JsSourceContext sourceContext, JsValueRef sourceUrl, JsParseScriptAttributes parseAttributes)
        {
            Native.ThrowIfError(Native.JsRun(script, sourceContext, sourceUrl, parseAttributes, out JsValueRef result));
            return result;
        }

        /// <summary>
        ///     Returns the exception that caused the runtime of the current context to be in the 
        ///     exception state and resets the exception state for that runtime.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     If the runtime of the current context is not in an exception state, this API will throw
        ///     <c>JsErrorInvalidArgument</c>. If the runtime is disabled, this will return an exception
        ///     indicating that the script was terminated, but it will not clear the exception (the 
        ///     exception will be cleared if the runtime is re-enabled using 
        ///     <c>EnableRuntimeExecution</c>).
        ///     </para>
        ///     <para>
        ///     Requires an active script context.
        ///     </para>
        /// </remarks>
        /// <returns>The exception for the runtime of the current context.</returns>
        public static JsValueRef GetAndClearException()
        {
            Native.ThrowIfError(Native.JsGetAndClearException(out JsValueRef reference));
            return reference;
        }

        /// <summary>
        ///     Sets the runtime of the current context to an exception state.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     If the runtime of the current context is already in an exception state, this API will 
        ///     throw <c>JsErrorInExceptionState</c>.
        ///     </para>
        ///     <para>
        ///     Requires an active script context.
        ///     </para>
        /// </remarks>
        /// <param name="exception">
        ///     The JavaScript exception to set for the runtime of the current context.
        /// </param>
        public static void SetException(JsValueRef exception)
        {
            Native.ThrowIfError(Native.JsSetException(exception));
        }

        /// <summary>
        ///     Adds a reference to a script context.
        /// </summary>
        /// <remarks>
        ///     Calling AddRef ensures that the context will not be freed until Release is called.
        /// </remarks>
        /// <returns>The object's new reference count.</returns>
        public uint AddRef()
        {
            Native.ThrowIfError(Native.JsContextAddRef(this, out uint count));
            return count;
        }

        /// <summary>
        ///     Releases a reference to a script context.
        /// </summary>
        /// <remarks>
        ///     Removes a reference to a context that was created by AddRef.
        /// </remarks>
        /// <returns>The object's new reference count.</returns>
        public uint Release()
        {
            Native.ThrowIfError(Native.JsContextRelease(this, out uint count));
            return count;
        }

        /// <summary>
		/// Sets a promise continuation callback function that is called by the context when a task
		/// needs to be queued for future execution
		/// </summary>
		/// <remarks>
		/// <para>
		/// Requires an active script context.
		/// </para>
		/// </remarks>
		/// <param name="promiseContinuationCallback">The callback function being set</param>
		/// <param name="callbackState">User provided state that will be passed back to the callback</param>
		public static void SetPromiseContinuationCallback(JsPromiseContinuationCallback promiseContinuationCallback, IntPtr callbackState)
        {
            Native.ThrowIfError(Native.JsSetPromiseContinuationCallback(promiseContinuationCallback, callbackState));
        }

        public override bool Equals(object obj) => reference.Equals(obj);

        public override int GetHashCode() => reference.GetHashCode();

        public static bool operator ==(JsContext a, JsContext b) => a.reference == b.reference;
        public static bool operator !=(JsContext a, JsContext b) => a.reference != b.reference;

        /// <summary>
        ///     A scope automatically sets a context to current and resets the original context
        ///     when disposed.
        /// </summary>
        public struct Scope : IDisposable
        {
            /// <summary>
            ///     The previous context.
            /// </summary>
            private readonly JsContext previousContext;

            /// <summary>
            ///     Whether the structure has been disposed.
            /// </summary>
            private bool disposed;

            /// <summary>
            ///     Initializes a new instance of the <see cref="Scope"/> struct. 
            /// </summary>
            /// <param name="context">The context to create the scope for.</param>
            public Scope(JsContext context)
            {
                disposed = false;
                previousContext = Current;
                Current = context;
            }

            /// <summary>
            ///     Disposes the scope and sets the previous context to current.
            /// </summary>
            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                Current = previousContext;
                disposed = true;
            }
        }
    }
}