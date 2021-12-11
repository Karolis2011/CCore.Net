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
    public class JsManagedFunctionTest : IClassFixture<BasicJsRuntimeFixture>
    {
        BasicJsRuntimeFixture fixture;
        public JsManagedFunctionTest(BasicJsRuntimeFixture fixture)
        {
            this.fixture = fixture;
        }


        [Fact]
        public void SimpleInvoke()
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);
            bool flag = false;
            var function = JsManagedFunction.Obtain<Action<int>>((i) =>
            {
                if (i > 10)
                    flag = true;
            });
            var global = JsObject.GlobalObject;
            global["test"] = function;

            JsContext.RunScript("test(20)");

            flag.Should().BeTrue();

            flag = false;

            JsContext.RunScript("test(5)");

            flag.Should().BeFalse();
        }

        [Theory]
        [InlineData(true, "true")]
        [InlineData("hi", "'hi'")]
        [InlineData(5, "5")]
        [InlineData(1.33, "1.33")]
        public void ReturnTypeTheory(object valueToReturn, string expectedJs)
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);
            Func<object> func = () => valueToReturn;
            var function = JsManagedFunction.Obtain(func);
            var global = JsObject.GlobalObject;
            global["test"] = function;

            var result = (JsBool)JsContext.RunScript($"test() == {expectedJs}");
            ((bool)result).Should().BeTrue();
        }

        [Fact]
        public void ForwardAndBackward()
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            var function = JsManagedFunction.Obtain<Func<bool>>(() => true);

            JsValueRef valRef = function;

            var managedValue = JsTypeMapper.FromRaw(valRef);
            managedValue.Should().BeSameAs(function);
        }
    }
}
