using CCore.Net.JsRt;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CCore.Net.Managed
{
    public class JsNativeFunction : JsFunction, IJsFreeable
    {

        protected JsRt.JsNativeFunction callDelegate;
        protected JsFinalizeCallback finalizeCallback;

        protected GCHandle callDelegateHandle;
        protected GCHandle finalizeCallbackHandle;
        protected GCHandle selfHandle;
        public bool IsFreed { get; protected set; } = false;

        protected JsNativeFunction() : base() { }

        public JsNativeFunction(JsRt.JsNativeFunction function, IntPtr callbackState) : base()
        {
            JsNativeFunctionInit(function, callbackState);
        }

        protected void JsNativeFunctionInit(JsRt.JsNativeFunction function, IntPtr callbackState)
        {
            callDelegate = function;
            finalizeCallback = (ptr) =>
                OnJsFinalize();
            callDelegateHandle = GCHandle.Alloc(callDelegate);
            finalizeCallbackHandle = GCHandle.Alloc(finalizeCallback);
            selfHandle = GCHandle.Alloc(this);
            jsValueRef = JsValueRef.CreateFunction(callDelegate, callbackState);
            var externalObject = JsValueRef.CreateExternalObject(IntPtr.Zero, finalizeCallback);
            externalObject.ExternalData = (IntPtr)selfHandle;
            SetInternalProperty(ExternalObjectPropertyName, externalObject);
        }

        protected void JsNativeFunctionInit(JsRt.JsNativeFunction function, IntPtr callbackState, string name)
        {
            callDelegate = function;
            finalizeCallback = (ptr) =>
                OnJsFinalize();
            callDelegateHandle = GCHandle.Alloc(callDelegate);
            finalizeCallbackHandle = GCHandle.Alloc(finalizeCallback);
            selfHandle = GCHandle.Alloc(this);

            jsValueRef = JsValueRef.CreateNamedFunction(name, callDelegate, callbackState);
            var externalObject = JsValueRef.CreateExternalObject(IntPtr.Zero, finalizeCallback);
            externalObject.ExternalData = (IntPtr)selfHandle;
            SetInternalProperty(ExternalObjectPropertyName, externalObject);
        }



        protected void OnJsFinalize()
        {
            if (!IsFreed)
            {
                callDelegateHandle.Free();
                finalizeCallbackHandle.Free();
                selfHandle.Free();
                IsFreed = true;
            }
        }

    }
}
