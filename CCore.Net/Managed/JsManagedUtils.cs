using CCore.Net.JsRt;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CCore.Net.Managed
{
    internal static class JsManagedUtils
    {
        internal static JsValueRef CreateErrorFromWrapperException(JsException exception)
        {
            JsErrorCode errorCode = exception.InnerException is JsException originalException ?
                originalException.ErrorCode : JsErrorCode.NoError;
            var description = Enum.GetName(typeof(JsErrorCode), errorCode);

            JsValueRef innerErrorValue = JsValueRef.CreateError(JsValueRef.From(description));
            innerErrorValue.SetIndexedProperty(JsValueRef.From("description"), JsValueRef.From(description));

            JsValueRef errorValue = JsValueRef.CreateError(JsValueRef.From(description));
            errorValue.SetIndexedProperty(JsValueRef.From("innerException"), innerErrorValue);
            return errorValue;
        }

        internal static Exception UnwrapException(Exception exception)
        {
            Exception originalException = exception;
            if (exception is TargetInvocationException targetInvocationException)
            {
                Exception innerException = targetInvocationException.InnerException;
                if (innerException != null)
                    originalException = innerException;
            }
            return originalException;
        }
    }
}
