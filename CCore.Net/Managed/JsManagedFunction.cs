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
    public class JsManagedFunction : JsManagedFunction<Delegate> { }


    public class JsManagedFunction<T> : InternalJsManagedFunction where T : Delegate
    {
        public static InternalJsManagedFunction ObtainUsingDelegate(Delegate @delegate)
        {
            var runtime = BasicJsRuntime.ActiveRuntime;
            if (runtime == null)
                throw new InvalidOperationException("No runtime present");
            var delegateType = @delegate.GetType();

            if (runtime.TryGetExistingManaged(@delegate, out var value))
            {
                if (value is InternalJsManagedFunction imf && delegateType.IsAssignableFrom(imf.Target.GetType()))
                    return imf;
            }
            var genericType = typeof(JsManagedFunction<>);
            var managedType = genericType.MakeGenericType(delegateType);
            return (InternalJsManagedFunction)Activator.CreateInstance(managedType, @delegate);
        }

        public static JsManagedFunction<D> Obtain<D>(D @delegate) where D : Delegate
        {
            var runtime = BasicJsRuntime.ActiveRuntime;
            if (runtime == null)
                throw new InvalidOperationException("No runtime present");
            if(runtime.TryGetExistingManaged(@delegate, out var value))
            {
                if(value is JsManagedFunction<D> managedFunction)
                    return managedFunction;

            }
            return new JsManagedFunction<D>(@delegate);
        }

        public static JsManagedFunction<D> Obtain<D>(D @delegate, string name) where D : Delegate
        {
            var runtime = BasicJsRuntime.ActiveRuntime;
            if (runtime == null)
                throw new InvalidOperationException("No runtime present");
            if (runtime.TryGetExistingManaged(@delegate, out var value))
            {
                if (value is JsManagedFunction<D> managedFunction)
                    return managedFunction;
            }
            return new JsManagedFunction<D>(@delegate, name);
        }

        public new T Target
        {
            get
            {
                return (T)base.Target;
            }
            private set
            {
                base.Target = value;
            }
        }

        protected JsManagedFunction() { }
        internal JsManagedFunction(T @delegate)
        {
            Target = @delegate;
            JsNativeFunctionInit(WrapDelegate(@delegate), IntPtr.Zero);
            runtime.TrackManaged(this, Target);
        }

        internal JsManagedFunction(T @delegate, string name)
        {
            Target = @delegate;
            JsNativeFunctionInit(WrapDelegate(@delegate), IntPtr.Zero, name);
            runtime.TrackManaged(this, Target);
        }
	}

    public class InternalJsManagedFunction : JsWrappedFunction
    {
        public Delegate Target { get; protected set; }

        protected InternalJsManagedFunction() { }
    }
}
