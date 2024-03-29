﻿using System;
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
    public struct JsValueRef
    {
        /// <summary>
        /// The reference.
        /// </summary>
        private readonly JsRef reference;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsValueRef"/> struct.
        /// </summary>
        /// <param name="reference">The reference.</param>
        private JsValueRef(JsRef reference) => this.reference = reference;

        /// <summary>
        ///     Gets an invalid value.
        /// </summary>
        public static JsValueRef Invalid => new JsValueRef(JsRef.Invalid);

        /// <summary>
        ///     Gets the value of <c>undefined</c> in the current script context.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        public static JsValueRef Undefined
        {
            get
            {
                Native.ThrowIfError(Native.JsGetUndefinedValue(out JsValueRef value));
                return value;
            }
        }

        /// <summary>
        ///     Gets the value of <c>null</c> in the current script context.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        public static JsValueRef Null
        {
            get
            {
                Native.ThrowIfError(Native.JsGetNullValue(out JsValueRef value));
                return value;
            }
        }

        /// <summary>
        ///     Gets the value of <c>true</c> in the current script context.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        public static JsValueRef True
        {
            get
            {
                Native.ThrowIfError(Native.JsGetTrueValue(out JsValueRef value));
                return value;
            }
        }

        /// <summary>
        ///     Gets the value of <c>false</c> in the current script context.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        public static JsValueRef False
        {
            get
            {
                Native.ThrowIfError(Native.JsGetFalseValue(out JsValueRef value));
                return value;
            }
        }

        /// <summary>
        ///     Gets the global object in the current script context.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        public static JsValueRef GlobalObject
        {
            get
            {
                Native.ThrowIfError(Native.JsGetGlobalObject(out JsValueRef value));
                return value;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the value is valid.
        /// </summary>
        public bool IsValid => reference.IsValid; 

        /// <summary>
        ///     Gets the JavaScript type of the value.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <returns>The type of the value.</returns>
        public JsValueType ValueType
        {
            get
            {
                Native.ThrowIfError(Native.JsGetValueType(this, out JsValueType type));
                return type;
            }
        }

        /// <summary>
        ///     Gets the length of a <c>String</c> value.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <returns>The length of the string.</returns>
        public int StringLength
        {
            get
            {
                Native.ThrowIfError(Native.JsGetStringLength(this, out int length));
                return length;
            }
        }

        /// <summary>
        ///     Gets or sets the prototype of an object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        public JsValueRef Prototype
        {
            get
            {
                Native.ThrowIfError(Native.JsGetPrototype(this, out JsValueRef prototypeReference));
                return prototypeReference;
            }

            set
            {
                Native.ThrowIfError(Native.JsSetPrototype(this, value));
            }
        }

        /// <summary>
        ///     Gets a value indicating whether an object is extensible or not.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        public bool IsExtensionAllowed
        {
            get
            {
                Native.ThrowIfError(Native.JsGetExtensionAllowed(this, out bool allowed));
                return allowed;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether an object is an external object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        public bool HasExternalData
        {
            get
            {
                Native.ThrowIfError(Native.JsHasExternalData(this, out bool hasExternalData));
                return hasExternalData;
            }
        }

        /// <summary>
        ///     Gets or sets the data in an external object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        public IntPtr ExternalData
        {
            get
            {
                Native.ThrowIfError(Native.JsGetExternalData(this, out IntPtr data));
                return data;
            }

            set
            {
                Native.ThrowIfError(Native.JsSetExternalData(this, value));
            }
        }

        /// <summary>
        ///     Creates a <c>Boolean</c> value from a <c>bool</c> value.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="value">The value to be converted.</param>
        /// <returns>The converted value.</returns>
        public static JsValueRef From(bool value)
        {
            Native.ThrowIfError(Native.JsBoolToBoolean(value, out JsValueRef reference));
            return reference;
        }

        /// <summary>
        ///     Creates a <c>Number</c> value from a <c>double</c> value.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="value">The value to be converted.</param>
        /// <returns>The new <c>Number</c> value.</returns>
        public static JsValueRef From(double value)
        {
            Native.ThrowIfError(Native.JsDoubleToNumber(value, out JsValueRef reference));
            return reference;
        }

        /// <summary>
        ///     Creates a <c>Number</c> value from a <c>int</c> value.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="value">The value to be converted.</param>
        /// <returns>The new <c>Number</c> value.</returns>
        public static JsValueRef From(int value)
        {
            Native.ThrowIfError(Native.JsIntToNumber(value, out JsValueRef reference));
            return reference;
        }

        /// <summary>
        ///     Creates a <c>String</c> value from a string pointer.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="value">The string  to convert to a <c>String</c> value.</param>
        /// <returns>The new <c>String</c> value.</returns>
        public static JsValueRef From(string value)
        {
            Native.ThrowIfError(Native.JsPointerToString(value, new UIntPtr((uint)value.Length), out JsValueRef reference));
            return reference;
        }

        /// <summary>
        ///     Creates a new <c>Object</c>.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <returns>The new <c>Object</c>.</returns>
        public static JsValueRef CreateObject()
        {
            Native.ThrowIfError(Native.JsCreateObject(out JsValueRef reference));
            return reference;
        }

        /// <summary>
        ///     Creates a new <c>Object</c> that stores some external data.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="data">External data that the object will represent. May be null.</param>
        /// <param name="finalizer">
        ///     A callback for when the object is finalized. May be null.
        /// </param>
        /// <returns>The new <c>Object</c>.</returns>
        public static JsValueRef CreateExternalObject(IntPtr data, JsFinalizeCallback finalizer)
        {
            Native.ThrowIfError(Native.JsCreateExternalObject(data, finalizer, out JsValueRef reference));
            return reference;
        }

        /// <summary>
        ///     Creates a new JavaScript function.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="function">The method to call when the function is invoked.</param>
        /// <returns>The new function object.</returns>
        public static JsValueRef CreateFunction(JsNativeFunction function)
        {
            Native.ThrowIfError(Native.JsCreateFunction(function, IntPtr.Zero, out JsValueRef reference));
            return reference;
        }

        /// <summary>
        ///     Creates a new JavaScript function.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="function">The method to call when the function is invoked.</param>
        /// <param name="callbackData">Data to be provided to all function callbacks.</param>
        /// <returns>The new function object.</returns>
        public static JsValueRef CreateFunction(JsNativeFunction function, IntPtr callbackData)
        {
            Native.ThrowIfError(Native.JsCreateFunction(function, callbackData, out JsValueRef reference));
            return reference;
        }

        /// <summary>
        ///     Creates a new JavaScript named function.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="name">Nane for this function</param>
        /// <param name="function">The method to call when the function is invoked.</param>
        /// <returns>The new function object.</returns>
        public static JsValueRef CreateNamedFunction(JsValueRef name, JsNativeFunction function)
        {
            Native.ThrowIfError(Native.JsCreateNamedFunction(name, function, IntPtr.Zero, out JsValueRef reference));
            return reference;
        }

        /// <summary>
        ///     Creates a new JavaScript named function.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="name">Nane for this function</param>
        /// <param name="function">The method to call when the function is invoked.</param>
        /// <returns>The new function object.</returns>
        public static JsValueRef CreateNamedFunction(string name, JsNativeFunction function)
        {
            return CreateNamedFunction(From(name), function);
        }

        /// <summary>
        ///     Creates a new JavaScript named function.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="name">Nane for this function</param>
        /// <param name="function">The method to call when the function is invoked.</param>
        /// <param name="callbackData">Data to be provided to all function callbacks.</param>
        /// <returns>The new function object.</returns>
        public static JsValueRef CreateNamedFunction(JsValueRef name, JsNativeFunction function, IntPtr callbackData)
        {
            Native.ThrowIfError(Native.JsCreateNamedFunction(name, function, callbackData, out JsValueRef reference));
            return reference;
        }

        /// <summary>
        ///     Creates a new JavaScript named function.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="name">Nane for this function</param>
        /// <param name="function">The method to call when the function is invoked.</param>
        /// <param name="callbackData">Data to be provided to all function callbacks.</param>
        /// <returns>The new function object.</returns>
        public static JsValueRef CreateNamedFunction(string name, JsNativeFunction function, IntPtr callbackData)
        {
            return CreateNamedFunction(From(name), function, callbackData);
        }


        /// <summary>
        ///     Creates a JavaScript array object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="length">The initial length of the array.</param>
        /// <returns>The new array object.</returns>
        public static JsValueRef CreateArray(uint length)
        {
            Native.ThrowIfError(Native.JsCreateArray(length, out JsValueRef reference));
            return reference;
        }

        /// <summary>
        ///     Creates a new JavaScript error object
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="message">Message for the error object.</param>
        /// <returns>The new error object.</returns>
        public static JsValueRef CreateError(JsValueRef message)
        {
            Native.ThrowIfError(Native.JsCreateError(message, out JsValueRef reference));
            return reference;
        }

        /// <summary>
        ///     Creates a new JavaScript error object
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="message">Message for the error object.</param>
        /// <returns>The new error object.</returns>
        public static JsValueRef CreateError(string message)
        {
            return CreateError(From(message));
        }

        /// <summary>
        ///     Creates a new JavaScript RangeError error object
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="message">Message for the error object.</param>
        /// <returns>The new error object.</returns>
        public static JsValueRef CreateRangeError(JsValueRef message)
        {
            Native.ThrowIfError(Native.JsCreateRangeError(message, out JsValueRef reference));
            return reference;
        }

        /// <summary>
        ///     Creates a new JavaScript RangeError error object
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="message">Message for the error object.</param>
        /// <returns>The new error object.</returns>
        public static JsValueRef CreateRangeError(string message)
        {
            return CreateRangeError(From(message));
        }

        /// <summary>
        ///     Creates a new JavaScript ReferenceError error object
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="message">Message for the error object.</param>
        /// <returns>The new error object.</returns>
        public static JsValueRef CreateReferenceError(JsValueRef message)
        {
            Native.ThrowIfError(Native.JsCreateReferenceError(message, out JsValueRef reference));
            return reference;
        }

        /// <summary>
        ///     Creates a new JavaScript ReferenceError error object
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="message">Message for the error object.</param>
        /// <returns>The new error object.</returns>
        public static JsValueRef CreateReferenceError(string message)
        {
            return CreateReferenceError(From(message));
        }

        /// <summary>
        ///     Creates a new JavaScript SyntaxError error object
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="message">Message for the error object.</param>
        /// <returns>The new error object.</returns>
        public static JsValueRef CreateSyntaxError(JsValueRef message)
        {
            Native.ThrowIfError(Native.JsCreateSyntaxError(message, out JsValueRef reference));
            return reference;
        }

        /// <summary>
        ///     Creates a new JavaScript SyntaxError error object
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="message">Message for the error object.</param>
        /// <returns>The new error object.</returns>
        public static JsValueRef CreateSyntaxError(string message)
        {
            return CreateSyntaxError(From(message));
        }

        /// <summary>
        ///     Creates a new JavaScript TypeError error object
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="message">Message for the error object.</param>
        /// <returns>The new error object.</returns>
        public static JsValueRef CreateTypeError(JsValueRef message)
        {
            Native.ThrowIfError(Native.JsCreateTypeError(message, out JsValueRef reference));
            return reference;
        }

        /// <summary>
        ///     Creates a new JavaScript TypeError error object
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="message">Message for the error object.</param>
        /// <returns>The new error object.</returns>
        public static JsValueRef CreateTypeError(string message)
        {
            return CreateTypeError(From(message));
        }

        /// <summary>
        ///     Creates a new JavaScript URIError error object
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="message">Message for the error object.</param>
        /// <returns>The new error object.</returns>
        public static JsValueRef CreateUriError(JsValueRef message)
        {
            Native.ThrowIfError(Native.JsCreateURIError(message, out JsValueRef reference));
            return reference;
        }

        /// <summary>
        ///     Creates a new JavaScript URIError error object
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="message">Message for the error object.</param>
        /// <returns>The new error object.</returns>
        public static JsValueRef CreateUriError(string message)
        {
            return CreateUriError(From(message));
        }

        /// <summary>
        ///     Creates a new JavaScript promise objeact that resolves when resolveFunction or rejectFunction are called.s
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <returns>The new promise object.</returns>
        public static JsValueRef CreatePromise(out JsValueRef resolveFunction, out JsValueRef rejectFunction)
        {
            Native.ThrowIfError(Native.JsCreatePromise(out JsValueRef promise, out resolveFunction, out rejectFunction));
            return promise;
        }

        /// <summary>
        ///     Adds a reference to the object.
        /// </summary>
        /// <remarks>
        ///     This only needs to be called on objects that are not going to be stored somewhere on 
        ///     the stack. Calling AddRef ensures that the JavaScript object the value refers to will not be freed 
        ///     until Release is called
        /// </remarks>
        /// <returns>The object's new reference count.</returns>
        public uint AddRef()
        {
            Native.ThrowIfError(Native.JsAddRef(this, out uint count));
            return count;
        }

        /// <summary>
        ///     Releases a reference to the object.
        /// </summary>
        /// <remarks>
        ///     Removes a reference that was created by AddRef.
        /// </remarks>
        /// <returns>The object's new reference count.</returns>
        public uint Release()
        {
            Native.ThrowIfError(Native.JsRelease(this, out uint count));
            return count;
        }

        /// <summary>
        ///     Retrieves the <c>bool</c> value of a <c>Boolean</c> value.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <returns>The converted value.</returns>
        public bool ToBoolean()
        {
            Native.ThrowIfError(Native.JsBooleanToBool(this, out bool value));
            return value;
        }

        /// <summary>
        ///     Retrieves the <c>double</c> value of a <c>Number</c> value.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     This function retrieves the value of a Number value. It will fail with 
        ///     <c>InvalidArgument</c> if the type of the value is not <c>Number</c>.
        ///     </para>
        ///     <para>
        ///     Requires an active script context.
        ///     </para>
        /// </remarks>
        /// <returns>The <c>double</c> value.</returns>
        public double ToDouble()
        {
            Native.ThrowIfError(Native.JsNumberToDouble(this, out double value));
            return value;
        }

        /// <summary>
        ///     Retrieves the <c>int</c> value of a <c>Number</c> value.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     This function retrieves the value of a Number value. It will fail with
        ///     <c>InvalidArgument</c> if the type of the value is not <c>Number</c>.
        ///     </para>
        ///     <para>
        ///     Requires an active script context.
        ///     </para>
        /// </remarks>
        /// <returns>The <c>int</c> value.</returns>
        public int ToInt32()
        {
            Native.ThrowIfError(Native.JsNumberToInt(this, out int value));
            return value;
        }

        /// <summary>
        ///     Retrieves the string pointer of a <c>String</c> value.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     This function retrieves the string pointer of a <c>String</c> value. It will fail with 
        ///     <c>InvalidArgument</c> if the type of the value is not <c>String</c>.
        ///     </para>
        ///     <para>
        ///     Requires an active script context.
        ///     </para>
        /// </remarks>
        /// <returns>The string.</returns>
        public new string ToString()
        {
            Native.ThrowIfError(Native.JsStringToPointer(this, out IntPtr buffer, out UIntPtr length));
            return Marshal.PtrToStringUni(buffer, (int)length);
        }

        /// <summary>
        ///     Converts the value to <c>Boolean</c> using regular JavaScript semantics.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <returns>The converted value.</returns>
        public JsValueRef ConvertToBoolean()
        {
            Native.ThrowIfError(Native.JsConvertValueToBoolean(this, out JsValueRef booleanReference));
            return booleanReference;
        }

        /// <summary>
        ///     Converts the value to <c>Number</c> using regular JavaScript semantics.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <returns>The converted value.</returns>
        public JsValueRef ConvertToNumber()
        {
            Native.ThrowIfError(Native.JsConvertValueToNumber(this, out JsValueRef numberReference));
            return numberReference;
        }

        /// <summary>
        ///     Converts the value to <c>String</c> using regular JavaScript semantics.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <returns>The converted value.</returns>
        public JsValueRef ConvertToString()
        {
            Native.ThrowIfError(Native.JsConvertValueToString(this, out JsValueRef stringReference));
            return stringReference;
        }

        /// <summary>
        ///     Converts the value to <c>Object</c> using regular JavaScript semantics.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <returns>The converted value.</returns>
        public JsValueRef ConvertToObject()
        {
            Native.ThrowIfError(Native.JsConvertValueToObject(this, out JsValueRef objectReference));
            return objectReference;
        }

        /// <summary>
        ///     Sets an object to not be extensible.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        public void PreventExtension()
        {
            Native.ThrowIfError(Native.JsPreventExtension(this));
        }


        /// <summary>
        ///     Gets the list of all properties on the object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <returns>An array of property names.</returns>
        public JsValueRef GetOwnPropertyNames()
        {
            Native.ThrowIfError(Native.JsGetOwnPropertyNames(this, out JsValueRef propertyNamesReference));
            return propertyNamesReference;
        }

        /// <summary>
        ///     Test if an object has a value at the specified index.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="index">The index to test.</param>
        /// <returns>Whether the object has an value at the specified index.</returns>
        public bool HasIndexedProperty(JsValueRef index)
        {
            Native.ThrowIfError(Native.JsHasIndexedProperty(this, index, out bool hasProperty));
            return hasProperty;
        }

        /// <summary>
        ///     Retrieve the value at the specified index of an object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="index">The index to retrieve.</param>
        /// <returns>The retrieved value.</returns>
        public JsValueRef GetIndexedProperty(JsValueRef index)
        {
            Native.ThrowIfError(Native.JsGetIndexedProperty(this, index, out JsValueRef propertyReference));
            return propertyReference;
        }

        /// <summary>
        ///     Set the value at the specified index of an object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="index">The index to set.</param>
        /// <param name="value">The value to set.</param>
        public void SetIndexedProperty(JsValueRef index, JsValueRef value)
        {
            Native.ThrowIfError(Native.JsSetIndexedProperty(this, index, value));
        }

        /// <summary>
        ///     Set the value at the specified index of an object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="index">The index to set.</param>
        /// <param name="value">The value to set.</param>
        public void SetIndexedProperty(int index, JsValueRef value)
        {
            SetIndexedProperty(From(index), value);
        }

        /// <summary>
        ///     Delete the value at the specified index of an object.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="index">The index to delete.</param>
        public void DeleteIndexedProperty(JsValueRef index)
        {
            Native.ThrowIfError(Native.JsDeleteIndexedProperty(this, index));
        }

        /// <summary>
        ///     Compare two JavaScript values for equality.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     This function is equivalent to the "==" operator in JavaScript.
        ///     </para>
        ///     <para>
        ///     Requires an active script context.
        ///     </para>
        /// </remarks>
        /// <param name="other">The object to compare.</param>
        /// <returns>Whether the values are equal.</returns>
        public bool Equals(JsValueRef other)
        {
            Native.ThrowIfError(Native.JsEquals(this, other, out bool equals));
            return equals;
        }

        /// <summary>
        ///     Compare two JavaScript values for strict equality.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     This function is equivalent to the "===" operator in JavaScript.
        ///     </para>
        ///     <para>
        ///     Requires an active script context.
        ///     </para>
        /// </remarks>
        /// <param name="other">The object to compare.</param>
        /// <returns>Whether the values are strictly equal.</returns>
        public bool StrictEquals(JsValueRef other)
        {
            Native.ThrowIfError(Native.JsStrictEquals(this, other, out bool equals));
            return equals;
        }

        /// <summary>
        ///     Invokes a function.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="arguments">The arguments to the call. First argument is this. Same as in js function `call()`</param>
        /// <returns>The <c>Value</c> returned from the function invocation, if any.</returns>
        internal JsValueRef CallFunction(params JsValueRef[] arguments)
        {

            if (arguments.Length > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException("arguments");
            }

            Native.ThrowIfError(Native.JsCallFunction(this, arguments, (ushort)arguments.Length, out JsValueRef returnReference));
            return returnReference;
        }
        
        /// <summary>
        ///     Invokes a function.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="thisArg"><c>this</c> function arg</param>
        /// <param name="arguments">Regular arguments to function</param>
        /// <returns>The <c>Value</c> returned from the function invocation, if any.</returns>
        public JsValueRef CallFunction(JsValueRef thisArg, params JsValueRef[] arguments)
        {
            var args = new JsValueRef[arguments.Length+ 1];
            arguments.CopyTo(args, 1);
            args[0] = thisArg;
            return CallFunction(args);
        }

        /// <summary>
        ///     Invokes a function as a constructor.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="arguments">The arguments to the call.</param>
        /// <returns>The <c>Value</c> returned from the function invocation.</returns>
        public JsValueRef ConstructObject(params JsValueRef[] arguments)
        {
            if (arguments.Length > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException("arguments");
            }
            Native.ThrowIfError(Native.JsConstructObject(this, arguments, (ushort)arguments.Length, out JsValueRef returnReference));
            return returnReference;
        }

        /// <summary>
        ///     Defines a new object's own property from a property descriptor.
        /// </summary>
        /// <remarks>
        ///     Requires an active script context.
        /// </remarks>
        /// <param name="key">The key (JavascriptString or JavascriptSymbol) to the property.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <returns>Whether the property was defined.</returns>
        public bool ObjectDefineProperty(JsValueRef key, JsValueRef propertyDescriptor)
        {
            Native.ThrowIfError(Native.JsObjectDefineProperty(this, key, propertyDescriptor, out bool result));
            return result;
        }


        //public static Func<JsValueRef, string> JsonStringify()
        //{
        //    var glob = GlobalObject;
        //    var func = glob.GetProperty("JSON").GetProperty("stringify");
        //    return (val) =>
        //    {
        //        var res = func.CallFunction(glob, val);
        //        return res.ToString();
        //    };
        //}
    }
}