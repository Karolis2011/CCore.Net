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
    public class JsBoolTest : IClassFixture<BasicJsRuntimeFixture>
    {
        BasicJsRuntimeFixture fixture;
        public JsBoolTest(BasicJsRuntimeFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void TrueAndFalse()
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            var jsFalse = JsBool.False;
            var jsTrue = JsBool.True;

            jsFalse.Should().Be((JsBool)false).And.NotBe((JsBool)true);
            jsTrue.Should().Be((JsBool)true).And.NotBe((JsBool)false);
        }

        [Theory]
        [InlineData("1", true)]
        [InlineData("0", false)]
        [InlineData("undefined", false)]
        [InlineData("null", false)]
        [InlineData("\"\"", false)]
        [InlineData("{}", false)]
        [InlineData("{a:5}", true)]
        [InlineData("[]", true)]
        [InlineData("\"Hello\"", true)]
        public void ConvertsToBool(string scriptElement, bool expected)
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            var result = (JsValue)JsContext.RunScript(scriptElement);
            var boolResult = result.ConvertToBoolean();

            ((bool)boolResult).Should().Be(expected);

        }
    }
}
