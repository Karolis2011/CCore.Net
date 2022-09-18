using CCore.Net.JsRt;
using CCore.Net.Managed;
using CCore.Net.Runtimes;
using CCore.Net.Test.Runtimes.Fixtures;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CCore.Net.Test.Wrapper
{
    public class JsNativeFunctionTest : IClassFixture<BasicJsRuntimeFixture>
    {
        BasicJsRuntimeFixture fixture;
        public JsNativeFunctionTest(BasicJsRuntimeFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void SimpleInvoke()
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            var function = new Managed.JsNativeFunction((calle, isConstructor, arguments, argCount, state) =>
            {
                return JsBool.True;
            }, IntPtr.Zero);

            JsObject.GlobalObject["test"] = function;

            var functionResult = (JsValue)JsContext.RunScript("test()");
            functionResult.Should().BeOfType<JsBool>();
            var boolResult = (JsBool)functionResult;
            boolResult.Should().Be(JsBool.True);

            var jsFunction = (JsValue)JsContext.RunScript("test");
            jsFunction.GetType().Should().BeAssignableTo<JsFunction>();
            var reconvertedFunction = (JsFunction)jsFunction;

            reconvertedFunction.Should().Be(function);
        }

        [Fact]
        public void GarbageCollection()
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            var function = new Managed.JsNativeFunction((calle, isConstructor, arguments, argCount, state) =>
            {
                return JsBool.True;
            }, IntPtr.Zero);

            fixture.Runtime.InternalRuntime.CollectGarbage();
            fixture.Runtime.InternalRuntime.CollectGarbage();

            function.IsFreed.Should().BeTrue();
        }

        [Fact]
        public void ForwardAndBackward()
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            var function = new Managed.JsNativeFunction((calle, isConstructor, arguments, argCount, state) =>
            {
                return JsBool.True;
            }, IntPtr.Zero);

            JsValueRef valRef = function;

            var managedValue = JsTypeMapper.FromRaw(valRef);
            managedValue.Should().BeSameAs(function);
        }
    }
}
