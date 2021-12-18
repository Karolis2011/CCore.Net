using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CCore.Net.JsRt
{
    /// <summary>
    ///     Native interfaces.
    /// </summary>
    public static class Native
    {
        /// <summary>
        /// Throws if a native method returns an error code.
        /// </summary>
        /// <param name="error">The error.</param>
        internal static void ThrowIfError(JsErrorCode error)
        {
            if (error != JsErrorCode.NoError)
            {
                switch (error)
                {
                    case JsErrorCode.InvalidArgument:
                        throw new JsUsageException(error, "Invalid argument.");

                    case JsErrorCode.NullArgument:
                        throw new JsUsageException(error, "Null argument.");

                    case JsErrorCode.NoCurrentContext:
                        throw new JsUsageException(error, "No current context.");

                    case JsErrorCode.InExceptionState:
                        throw new JsUsageException(error, "Runtime is in exception state.");

                    case JsErrorCode.NotImplemented:
                        throw new JsUsageException(error, "Method is not implemented.");

                    case JsErrorCode.WrongThread:
                        throw new JsUsageException(error, "Runtime is active on another thread.");

                    case JsErrorCode.RuntimeInUse:
                        throw new JsUsageException(error, "Runtime is in use.");

                    case JsErrorCode.BadSerializedScript:
                        throw new JsUsageException(error, "Bad serialized script.");

                    case JsErrorCode.InDisabledState:
                        throw new JsUsageException(error, "Runtime is disabled.");

                    case JsErrorCode.CannotDisableExecution:
                        throw new JsUsageException(error, "Cannot disable execution.");

                    case JsErrorCode.AlreadyDebuggingContext:
                        throw new JsUsageException(error, "Context is already in debug mode.");

                    case JsErrorCode.HeapEnumInProgress:
                        throw new JsUsageException(error, "Heap enumeration is in progress.");

                    case JsErrorCode.ArgumentNotObject:
                        throw new JsUsageException(error, "Argument is not an object.");

                    case JsErrorCode.InProfileCallback:
                        throw new JsUsageException(error, "In a profile callback.");

                    case JsErrorCode.InThreadServiceCallback:
                        throw new JsUsageException(error, "In a thread service callback.");

                    case JsErrorCode.CannotSerializeDebugScript:
                        throw new JsUsageException(error, "Cannot serialize a debug script.");

                    case JsErrorCode.AlreadyProfilingContext:
                        throw new JsUsageException(error, "Already profiling this context.");

                    case JsErrorCode.IdleNotEnabled:
                        throw new JsUsageException(error, "Idle is not enabled.");

                    case JsErrorCode.OutOfMemory:
                        throw new JsEngineException(error, "Out of memory.");

                    case JsErrorCode.ScriptException:
                        {
                            JsErrorCode innerError = JsGetAndClearException(out JsValueRef errorObject);
                            if (innerError == JsErrorCode.InDisabledState)
                            {
                                var runtime = JsContext.Current.Runtime;
                                runtime.Disabled = false;
                                JsGetAndClearException(out JsValueRef _);
                                throw new JsTerminationException(innerError);
                            }
                            if (innerError != JsErrorCode.NoError)
                                throw new JsFatalException(innerError);
                            throw JsScriptException.FromError(error, errorObject, "Script error");
                        }

                    case JsErrorCode.ScriptCompile:
                        {
                            JsErrorCode innerError = JsGetAndClearException(out JsValueRef errorObject);

                            if (innerError == JsErrorCode.InDisabledState)
                            {
                                var runtime = JsContext.Current.Runtime;
                                runtime.Disabled = false;
                                JsGetAndClearException(out JsValueRef _);
                                throw new JsTerminationException(innerError);
                            }
                            if (innerError != JsErrorCode.NoError)
                                throw new JsFatalException(innerError);
                            throw JsScriptException.FromError(error, errorObject, "Compile error");
                        }

                    case JsErrorCode.ScriptTerminated:
                        throw new JsTerminationException(error);

                    case JsErrorCode.ScriptEvalDisabled:
                        throw new JsScriptException(error, JsValueRef.Invalid, "Eval of strings is disabled in this runtime.");

                    case JsErrorCode.Fatal:
                        throw new JsFatalException(error);

                    default:
                        throw new JsFatalException(error);
                }
            }
        }

        const string DllName = "ChakraCore";

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreateRuntime(JsRuntimeAttributes attributes, JsThreadServiceCallback threadService, out JsRuntime runtime);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCollectGarbage(JsRuntime handle);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsDisposeRuntime(JsRuntime handle);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetRuntimeMemoryUsage(JsRuntime runtime, out UIntPtr memoryUsage);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetRuntimeMemoryLimit(JsRuntime runtime, out UIntPtr memoryLimit);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsSetRuntimeMemoryLimit(JsRuntime runtime, UIntPtr memoryLimit);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsSetRuntimeMemoryAllocationCallback(JsRuntime runtime, IntPtr callbackState, JsMemoryAllocationCallback allocationCallback);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsSetRuntimeBeforeCollectCallback(JsRuntime runtime, IntPtr callbackState, JsBeforeCollectCallback beforeCollectCallback);

        [DllImport(DllName, EntryPoint = "JsAddRef")]
        internal static extern JsErrorCode JsContextAddRef(JsContext reference, out uint count);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsAddRef(JsValueRef reference, out uint count);

        [DllImport(DllName, EntryPoint = "JsRelease")]
        internal static extern JsErrorCode JsContextRelease(JsContext reference, out uint count);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsRelease(JsValueRef reference, out uint count);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreateContext(JsRuntime runtime, out JsContext newContext);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetCurrentContext(out JsContext currentContext);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsSetCurrentContext(JsContext context);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetRuntime(JsContext context, out JsRuntime runtime);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsIdle(out uint nextIdleTick);


        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetUndefinedValue(out JsValueRef undefinedValue);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetNullValue(out JsValueRef nullValue);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetTrueValue(out JsValueRef trueValue);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetFalseValue(out JsValueRef falseValue);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsBoolToBoolean(bool value, out JsValueRef booleanValue);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsBooleanToBool(JsValueRef booleanValue, out bool boolValue);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsConvertValueToBoolean(JsValueRef value, out JsValueRef booleanValue);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetValueType(JsValueRef value, out JsValueType type);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsDoubleToNumber(double doubleValue, out JsValueRef value);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsIntToNumber(int intValue, out JsValueRef value);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsNumberToDouble(JsValueRef value, out double doubleValue);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsConvertValueToNumber(JsValueRef value, out JsValueRef numberValue);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetStringLength(JsValueRef sringValue, out int length);

        [DllImport(DllName, CharSet = CharSet.Unicode)]
        internal static extern JsErrorCode JsPointerToString(string value, UIntPtr stringLength, out JsValueRef stringValue);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsStringToPointer(JsValueRef value, out IntPtr stringValue, out UIntPtr stringLength);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsConvertValueToString(JsValueRef value, out JsValueRef stringValue);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetGlobalObject(out JsValueRef globalObject);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreateObject(out JsValueRef obj);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreateExternalObject(IntPtr data, JsFinalizeCallback finalizeCallback, out JsValueRef obj);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsConvertValueToObject(JsValueRef value, out JsValueRef obj);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetPrototype(JsValueRef obj, out JsValueRef prototypeObject);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsSetPrototype(JsValueRef obj, JsValueRef prototypeObject);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetExtensionAllowed(JsValueRef obj, out bool value);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsPreventExtension(JsValueRef obj);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetOwnPropertyNames(JsValueRef obj, out JsValueRef propertyNames);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsHasIndexedProperty(JsValueRef obj, JsValueRef index, out bool result);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetIndexedProperty(JsValueRef obj, JsValueRef index, out JsValueRef result);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsSetIndexedProperty(JsValueRef obj, JsValueRef index, JsValueRef value);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsDeleteIndexedProperty(JsValueRef obj, JsValueRef index);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsEquals(JsValueRef obj1, JsValueRef obj2, out bool result);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsStrictEquals(JsValueRef obj1, JsValueRef obj2, out bool result);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsHasExternalData(JsValueRef obj, out bool value);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetExternalData(JsValueRef obj, out IntPtr externalData);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsSetExternalData(JsValueRef obj, IntPtr externalData);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreateArray(uint length, out JsValueRef result);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCallFunction(JsValueRef function, JsValueRef[] arguments, ushort argumentCount, out JsValueRef result);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsConstructObject(JsValueRef function, JsValueRef[] arguments, ushort argumentCount, out JsValueRef result);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreateFunction(JsNativeFunction nativeFunction, IntPtr externalData, out JsValueRef function);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreateError(JsValueRef message, out JsValueRef error);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreateRangeError(JsValueRef message, out JsValueRef error);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreateReferenceError(JsValueRef message, out JsValueRef error);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreateSyntaxError(JsValueRef message, out JsValueRef error);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreateTypeError(JsValueRef message, out JsValueRef error);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreateURIError(JsValueRef message, out JsValueRef error);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsHasException(out bool hasException);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetAndClearException(out JsValueRef exception);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsSetException(JsValueRef exception);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsDisableRuntimeExecution(JsRuntime runtime);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsEnableRuntimeExecution(JsRuntime runtime);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsIsRuntimeExecutionDisabled(JsRuntime runtime, out bool isDisabled);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsSetObjectBeforeCollectCallback(JsValueRef reference, IntPtr callbackState, JsObjectBeforeCollectCallback beforeCollectCallback);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreateNamedFunction(JsValueRef name, JsNativeFunction nativeFunction, IntPtr callbackState, out JsValueRef function);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreateArrayBuffer(uint byteLength, out JsValueRef result);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreateTypedArray(JsTypedArrayType arrayType, JsValueRef arrayBuffer, uint byteOffset,
            uint elementLength, out JsValueRef result);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreateDataView(JsValueRef arrayBuffer, uint byteOffset, uint byteOffsetLength, out JsValueRef result);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetArrayBufferStorage(JsValueRef arrayBuffer, out IntPtr buffer, out uint bufferLength);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetTypedArrayStorage(JsValueRef typedArray, out IntPtr buffer, out uint bufferLength, out JsTypedArrayType arrayType, out int elementSize);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetDataViewStorage(JsValueRef dataView, out IntPtr buffer, out uint bufferLength);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreateSymbol(JsValueRef description, out JsValueRef symbol);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetOwnPropertySymbols(JsValueRef obj, out JsValueRef propertySymbols);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsNumberToInt(JsValueRef value, out int intValue);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsSetIndexedPropertiesToExternalData(JsValueRef obj, IntPtr data, JsTypedArrayType arrayType, uint elementLength);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetIndexedPropertiesExternalData(JsValueRef obj, IntPtr data, out JsTypedArrayType arrayType, out uint elementLength);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsHasIndexedPropertiesExternalData(JsValueRef obj, out bool value);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsInstanceOf(JsValueRef obj, JsValueRef constructor, out bool result);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreateExternalArrayBuffer(IntPtr data, uint byteLength, JsFinalizeCallback finalizeCallback, IntPtr callbackState, out JsValueRef result);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetTypedArrayInfo(JsValueRef typedArray, out JsTypedArrayType arrayType, out JsValueRef arrayBuffer, out uint byteOffset, out uint byteLength);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetContextOfObject(JsValueRef obj, out JsContext context);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetContextData(JsContext context, out IntPtr data);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsSetContextData(JsContext context, IntPtr data);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsSetPromiseContinuationCallback(
            JsPromiseContinuationCallback promiseContinuationCallback, IntPtr callbackState);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsCreatePromise(out JsValueRef promise, out JsValueRef resolveFunction, out JsValueRef rejectFunction);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetPromiseState(JsValueRef promise, out JsPromiseState state);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsGetPromiseResult(JsValueRef promise, out JsValueRef result);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsSetHostPromiseRejectionTracker(
           JsHostPromiseRejectionTrackerCallback promiseRejectionTrackerCallback, IntPtr callbackState);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsParse(JsValueRef script, JsSourceContext sourceContext, JsValueRef sourceUrl, JsParseScriptAttributes parseAttributes, out JsValueRef result);

        [DllImport(DllName)]
        internal static extern JsErrorCode JsRun(JsValueRef script, JsSourceContext sourceContext, JsValueRef sourceUrl, JsParseScriptAttributes parseAttributes, out JsValueRef result);

        /// <summary>
        ///     Defines a new object's own property from a property descriptor.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="obj">The object that has the property.</param>
        /// <param name="key">The key (JavascriptString or JavascriptSymbol) to the property.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="result">Whether the property was defined.</param>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        [DllImport(DllName)]
        internal static extern JsErrorCode JsObjectDefineProperty(JsValueRef obj, JsValueRef key, JsValueRef propertyDescriptor, out bool result);


        /// <summary>
        ///     Starts debugging in the given runtime.
        /// </summary>
        /// <param name="runtime">Runtime to put into debug mode.</param>
        /// <param name="debugEventCallback">Registers a callback to be called on every JsDiagDebugEvent.</param>
        /// <param name="callbackState">User provided state that will be passed back to the callback.</param>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     The runtime should be active on the current thread and should not be in debug state.
        /// </remarks>
        [DllImport(DllName)]
        internal static extern JsErrorCode JsDiagStartDebugging(JsRuntime runtime, JsDiagDebugEventCallback debugEventCallback, IntPtr callbackState);

        /// <summary>
        ///     Stops debugging in the given runtime.
        /// </summary>
        /// <param name="runtime">Runtime to stop debugging.</param>
        /// <param name="callbackState">User provided state that was passed in JsDiagStartDebugging.</param>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     The runtime should be active on the current thread and in debug state.
        /// </remarks>
        [DllImport(DllName)]
        internal static extern JsErrorCode JsDiagStopDebugging(JsRuntime runtime, out IntPtr callbackState);

        /// <summary>
        ///     Request the runtime to break on next JavaScript statement.
        /// </summary>
        /// <param name="runtime">Runtime to request break.</param>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     The runtime should be in debug state. This API can be called from another runtime.
        /// </remarks>
        [DllImport(DllName)]
        internal static extern JsErrorCode JsDiagRequestAsyncBreak(JsRuntime runtime);

        /// <summary>
        ///     List all breakpoints in the current runtime.
        /// </summary>
        /// <param name="breakpoints">Array of breakpoints.</param>
        /// <remarks>
        ///     <para>
        ///     [{
        ///         "breakpointId" : 1,
        ///         "scriptId" : 1,
        ///         "line" : 0,
        ///         "column" : 62
        ///     }]
        ///     </para>
        /// </remarks>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     The current runtime should be in debug state. This API can be called when runtime is at a break or running.
        /// </remarks>
        [DllImport(DllName)]
        internal static extern JsErrorCode JsDiagGetBreakpoints(out JsValueRef breakpoints);

        /// <summary>
        ///     Sets breakpoint in the specified script at give location.
        /// </summary>
        /// <param name="scriptId">Id of script from JsDiagGetScripts or JsDiagGetSource to put breakpoint.</param>
        /// <param name="lineNumber">0 based line number to put breakpoint.</param>
        /// <param name="columnNumber">0 based column number to put breakpoint.</param>
        /// <param name="breakpoint">Breakpoint object with id, line and column if success.</param>
        /// <remarks>
        ///     <para>
        ///     {
        ///         "breakpointId" : 1,
        ///         "line" : 2,
        ///         "column" : 4
        ///     }
        ///     </para>
        /// </remarks>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     The current runtime should be in debug state. This API can be called when runtime is at a break or running.
        /// </remarks>
        [DllImport(DllName)]
        internal static extern JsErrorCode JsDiagGetBreakpoints(uint scriptId, uint lineNumber, uint columnNumber, out JsValueRef breakpoint);

        /// <summary>
        ///     Remove a breakpoint.
        /// </summary>
        /// <param name="breakpointId">Breakpoint id returned from JsDiagSetBreakpoint.</param>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     The current runtime should be in debug state. This API can be called when runtime is at a break or running.
        /// </remarks>
        [DllImport(DllName)]
        internal static extern JsErrorCode JsDiagRemoveBreakpoint(uint scriptIdbreakpointId);

        /// <summary>
        ///     Sets break on exception handling.
        /// </summary>
        /// <param name="runtime">Runtime to set break on exception attributes.</param>
        /// <param name="exceptionAttributes">Mask of JsDiagBreakOnExceptionAttributes to set.</param>
        /// <remarks>
        ///     <para>
        ///         If this API is not called the default value is set to JsDiagBreakOnExceptionAttributeUncaught in the runtime.
        ///     </para>
        /// </remarks>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     The runtime should be in debug state. This API can be called from another runtime.
        /// </remarks>
        [DllImport(DllName)]
        internal static extern JsErrorCode JsDiagSetBreakOnException(JsRuntime runtime, JsDiagBreakOnExceptionAttributes exceptionAttributes);

        /// <summary>
        ///     Gets break on exception setting.
        /// </summary>
        /// <param name="runtime">Runtime from which to get break on exception attributes, should be in debug mode.</param>
        /// <param name="exceptionAttributes">Mask of JsDiagBreakOnExceptionAttributes.</param>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     The runtime should be in debug state. This API can be called from another runtime.
        /// </remarks>
        [DllImport(DllName)]
        internal static extern JsErrorCode JsDiagGetBreakOnException(JsRuntime runtime, out JsDiagBreakOnExceptionAttributes exceptionAttributes);

        /// <summary>
        ///     Sets the step type in the runtime after a debug break.
        /// </summary>
        /// <remarks>
        ///     Requires to be at a debug break.
        /// </remarks>
        /// <param name="stepType">Type of JsDiagStepType.</param>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     The current runtime should be in debug state. This API can only be called when runtime is at a break.
        /// </remarks>
        [DllImport(DllName)]
        internal static extern JsErrorCode JsDiagSetStepType(JsDiagStepType stepType);

        /// <summary>
        ///     Gets list of scripts.
        /// </summary>
        /// <param name="scriptsArray">Array of script objects.</param>
        /// <remarks>
        ///     <para>
        ///     [{
        ///         "scriptId" : 2,
        ///         "fileName" : "c:\\Test\\Test.js",
        ///         "lineCount" : 4,
        ///         "sourceLength" : 111
        ///       }, {
        ///         "scriptId" : 3,
        ///         "parentScriptId" : 2,
        ///         "scriptType" : "eval code",
        ///         "lineCount" : 1,
        ///         "sourceLength" : 12
        ///     }]
        ///     </para>
        /// </remarks>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     The current runtime should be in debug state. This API can be called when runtime is at a break or running.
        /// </remarks>
        [DllImport(DllName)]
        internal static extern JsErrorCode JsDiagGetScripts(out JsValueRef scriptsArray);

        /// <summary>
        ///     Gets source for a specific script identified by scriptId from JsDiagGetScripts.
        /// </summary>
        /// <param name="scriptId">Id of the script.</param>
        /// <param name="source">Source object.</param>
        /// <remarks>
        ///     <para>
        ///     {
        ///         "scriptId" : 1,
        ///         "fileName" : "c:\\Test\\Test.js",
        ///         "lineCount" : 12,
        ///         "sourceLength" : 15154,
        ///         "source" : "var x = 1;"
        ///     }
        ///     </para>
        /// </remarks>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     The current runtime should be in debug state. This API can be called when runtime is at a break or running.
        /// </remarks>
        [DllImport(DllName)]
        internal static extern JsErrorCode JsDiagGetSource(uint scriptId, out JsValueRef source);

        /// <summary>
        ///     Gets the source information for a function object.
        /// </summary>
        /// <param name="function">JavaScript function.</param>
        /// <param name="functionPosition">Function position - scriptId, start line, start column, line number of first statement, column number of first statement.</param>
        /// <remarks>
        ///     <para>
        ///     {
        ///         "scriptId" : 1,
        ///         "fileName" : "c:\\Test\\Test.js",
        ///         "line" : 1,
        ///         "column" : 2,
        ///         "firstStatementLine" : 6,
        ///         "firstStatementColumn" : 0
        ///     }
        ///     </para>
        /// </remarks>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     This API can be called when runtime is at a break or running.
        /// </remarks>
        [DllImport(DllName)]
        internal static extern JsErrorCode JsDiagGetFunctionPosition(JsValueRef function, out JsValueRef functionPosition);

        /// <summary>
        ///     Gets the stack trace information.
        /// </summary>
        /// <param name="stackTrace">Stack trace information.</param>
        /// <remarks>
        ///     <para>
        ///     [{
        ///         "index" : 0,
        ///         "scriptId" : 2,
        ///         "line" : 3,
        ///         "column" : 0,
        ///         "sourceLength" : 9,
        ///         "sourceText" : "var x = 1",
        ///         "functionHandle" : 1
        ///     }]
        ///    </para>
        /// </remarks>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     The current runtime should be in debug state. This API can only be called when runtime is at a break.
        /// </remarks>
        [DllImport(DllName)]
        internal static extern JsErrorCode JsDiagGetStackTrace(out JsValueRef stackTrace);

        /// <summary>
        ///     Gets the list of properties corresponding to the frame.
        /// </summary>
        /// <param name="stackFrameIndex">Index of stack frame from JsDiagGetStackTrace.</param>
        /// <param name="properties">Object of properties array (properties, scopes and globals).</param>
        /// <remarks>
        ///     <para>
        ///     propertyAttributes is a bit mask of
        ///         NONE = 0x1,
        ///         HAVE_CHILDRENS = 0x2,
        ///         READ_ONLY_VALUE = 0x4,
        ///         IN_TDZ = 0x8,
        ///     </para>
        ///     <para>
        ///     {
        ///         "thisObject": {
        ///             "name": "this",
        ///             "type" : "object",
        ///             "className" : "Object",
        ///             "display" : "{...}",
        ///             "propertyAttributes" : 1,
        ///             "handle" : 306
        ///         },
        ///         "exception" : {
        ///             "name" : "{exception}",
        ///             "type" : "object",
        ///             "display" : "'a' is undefined",
        ///             "className" : "Error",
        ///             "propertyAttributes" : 1,
        ///             "handle" : 307
        ///         }
        ///         "arguments" : {
        ///             "name" : "arguments",
        ///             "type" : "object",
        ///             "display" : "{...}",
        ///             "className" : "Object",
        ///             "propertyAttributes" : 1,
        ///             "handle" : 190
        ///         },
        ///         "returnValue" : {
        ///             "name" : "[Return value]",
        ///             "type" : "undefined",
        ///             "propertyAttributes" : 0,
        ///             "handle" : 192
        ///         },
        ///         "functionCallsReturn" : [{
        ///                 "name" : "[foo1 returned]",
        ///                 "type" : "number",
        ///                 "value" : 1,
        ///                 "propertyAttributes" : 2,
        ///                 "handle" : 191
        ///             }
        ///         ],
        ///         "locals" : [],
        ///         "scopes" : [{
        ///                 "index" : 0,
        ///                 "handle" : 193
        ///             }
        ///         ],
        ///         "globals" : {
        ///             "handle" : 194
        ///         }
        ///     }
        ///     </para>
        /// </remarks>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     The current runtime should be in debug state. This API can only be called when runtime is at a break.
        /// </remarks>
        [DllImport(DllName)]
        internal static extern JsErrorCode JsDiagGetStackProperties(uint stackFrameIndex, out JsValueRef properties);


        // TODO:
        // https://github.com/Microsoft/ChakraCore/issues/4324
    }

}