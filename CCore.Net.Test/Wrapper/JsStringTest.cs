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
    public class JsStringTest : IClassFixture<BasicJsRuntimeFixture>
    {
        BasicJsRuntimeFixture fixture;

        public JsStringTest(BasicJsRuntimeFixture fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [InlineData("Test")]
        [InlineData("❤😥")]
        [InlineData("Ąžerty")]
        public void ConversionTheory(string initial)
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            var jsString = (JsString)initial;
            var GlobalObject = JsObject.GlobalObject;

            GlobalObject["str"] = jsString;
            GlobalObject["res"] = (JsNumber)0;

            JsContext.RunScript("res = str.length");

            var jsLength = (JsNumber)GlobalObject["res"];

            ((int)jsLength).Should().NotBe(0).And.Be(initial.Length);

            var extracted = (string)jsString;
            extracted.Should().Be(initial);
        }

        [Theory]
        [InlineData("Test", "Here")]
        [InlineData("❤😥", "😁🎎")]
        [InlineData("Ąžertyų", "ČĖĮŠ")]
        public void ConcatinationTheory(string a, string b)
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            var GlobalObject = JsObject.GlobalObject;
            GlobalObject["a"] = (JsString)a;
            GlobalObject["b"] = (JsString)b;
            GlobalObject["res"] = JsValueRef.Null;

            JsContext.RunScript("res = a + b");

            var result = (JsString)GlobalObject["res"];

            ((string)result).Should().Be(a + b);
        }
    }
}
