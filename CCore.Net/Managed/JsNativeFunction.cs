using CCore.Net.JsRt;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CCore.Net.Managed
{
    public class JsNativeFunction : JsFunction
    {
        protected JsRt.JsNativeFunction callDelegate;
        protected JsFinalizeCallback finalizeCallback;

        protected GCHandle callDelegateHandle;
        protected GCHandle finalizeCallbackHandle;
        protected GCHandle selfHandle;
        public bool IsFreeed { get; protected set; } = false;

        protected JsNativeFunction() { }

        public JsNativeFunction(JsRt.JsNativeFunction function, IntPtr callbackState)
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
            SetNonEnumerableProperty(ExternalObjectPropertyName, externalObject);
        }



        protected void OnJsFinalize()
        {
            if (!IsFreeed)
            {
                callDelegateHandle.Free();
                finalizeCallbackHandle.Free();
                selfHandle.Free();
                IsFreeed = true;
            }
        }

    }
}
