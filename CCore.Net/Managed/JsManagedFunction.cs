using CCore.Net.JsRt;
using CCore.Net.Runtimes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace CCore.Net.Managed
{
    public class JsManagedFunction : JsNativeFunction
    {

        public JsManagedFunction(Delegate @delegate)
        {
            JsNativeFunctionInit(WrapDelegate(@delegate), IntPtr.Zero);
        }

        private JsRt.JsNativeFunction WrapDelegate(Delegate @delegate)
        {
            return (callee, isConstructCall, args, argCount, callbackData) =>
            {
                var methodInfo = @delegate.GetMethodInfo();
                var parameters = methodInfo.GetParameters();
                try
                {
                    if (!IsCompatibleSignature(args.Select(v => v.ValueType).ToArray(), args, parameters, out object[] processedArgs))
                    {
                        throw new Exception("Method signature is incompatible with passed arguments.");
                    }
                    object result = @delegate.DynamicInvoke(processedArgs);

                    return JsTypeMapper.ToScript(result);
                }
                catch (Exception e)
                {
                    ProcessException(e);
                }
                return JsValueRef.Undefined;
            };
        }

        private void ProcessException(Exception exception)
        {
            exception = UnwrapException(exception);
            if(exception is JsException jsrte)
            {
                runtime.SetException(CreateErrorFromWrapperException(jsrte));
            } else
            {
                runtime.SetException(new JsError($"Error invoking managed function: {exception.Message}"));
            }
        }

        private static JsValueRef CreateErrorFromWrapperException(JsException exception)
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

        private static Exception UnwrapException(Exception exception)
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


        private bool IsCompatibleSignature(JsValueType[] argTypes, JsValueRef[] args, ParameterInfo[] parameters, out object[] mappedParameters)
		{
            mappedParameters = new object[parameters.Length];
            var mp = mappedParameters;
			int argPos = 1; //Start 

            var globalTypeSwitch = new Dictionary<Type, Action<int>>()
            {
                { typeof(JsContext), (i) => mp[i] = runtime.InternalContext },
                { typeof(IJsRuntime), (i) => mp[i] = runtime },
                { typeof(BasicJsRuntime), (i) => mp[i] = runtime },
            };


            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                if(globalTypeSwitch.ContainsKey(param.ParameterType))
                {
                    globalTypeSwitch[param.ParameterType](i);
                } else
                {
                    if (argPos == args.Length)
                        return false;
                    try
                    {
                        mp[i] = JsTypeMapper.ToHost(args[argPos], param.ParameterType);
                        argPos++;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }

            }
            mappedParameters = mp;
            return true;
        }
	}
}
