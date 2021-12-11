using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CCore.Net.Managed
{
    public class JsManagedMethod<T> : JsManagedFunction
    {
        public new T Target { get; private set; }
        internal JsManagedMethod(MethodInfo[] methodGroup, T obj, string name)
        {
            Target = obj;
            JsNativeFunctionInit(WrapDelegate(methodGroup, obj), IntPtr.Zero, name);
        }
    }
    public class JsManagedMethod : JsManagedFunction
    {
        public new object Target { get; private set; }
        internal JsManagedMethod(MethodInfo[] methodGroup, object obj, string name)
        {
            Target = obj;
            JsNativeFunctionInit(WrapDelegate(methodGroup, obj), IntPtr.Zero, name);
        }
    }
}
