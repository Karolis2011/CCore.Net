using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CCore.Net.Managed
{
    public class JsManagedMethod<T> : JsManagedMethod
    {
        public new T Target { get => (T)base.Target; protected set  => base.Target = value; }
        internal JsManagedMethod(MethodInfo[] methodGroup, T obj, string name)
        {
            Target = obj;
            JsNativeFunctionInit(WrapDelegate(methodGroup, obj), IntPtr.Zero, name);
        }
    }
    public class JsManagedMethod : JsWrappedFunction
    {
        public object Target { get; protected set; }

        protected JsManagedMethod() : base() { }
        internal JsManagedMethod(MethodInfo[] methodGroup, object obj, string name)
        {
            Target = obj;
            JsNativeFunctionInit(WrapDelegate(methodGroup, obj), IntPtr.Zero, name);
        }
    }
}
