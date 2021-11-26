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
    public class JsFunctionTest : IClassFixture<BasicJsRuntimeFixture>
    {
        BasicJsRuntimeFixture fixture;
        public JsFunctionTest(BasicJsRuntimeFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void SimpleInvoke()
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            var function = (JsValue)JsContext.RunScript("() => true");

            function.Should().BeOfType<JsFunction>();
            var fn = (JsFunction)function;
            fn.Should().NotBeNull();

            var result = fn.Invoke(JsNull.Null);
            result.Should().BeOfType<JsBool>();
            var boolResult = (JsBool)result;

            boolResult.Should().Be(JsBool.True);
        }
    }
}
