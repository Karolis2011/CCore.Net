using ChakraCore.Net;
using ChakraCore.Net.JsRt;
using System;
using Xunit;
using ChakraCore.Net.Managed;
using FluentAssertions;
using ChakraCore.Net.Test.Runtimes.Fixtures;
using ChakraCore.Net.Runtimes;

namespace ChakraCore.Net.Test.Wrapper
{
    public class JsNumberTest : IClassFixture<BasicJsRuntimeFixture>
    {
        BasicJsRuntimeFixture fixture;

        public JsNumberTest(BasicJsRuntimeFixture fixture)
        {
            this.fixture = fixture;
        }

        
        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(120)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void ConversionTheoryInt(int initial)
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            var number = (JsNumber)initial;
            var GlobalObject = JsObject.GlobalObject;
            GlobalObject["num"] = number;
            GlobalObject["r1"] = JsValueRef.Null;
            GlobalObject["r2"] = JsValueRef.Null;

            JsContext.RunScript($"r1 = (num == {initial}); r2 = num");

            var r1 = (JsBool)GlobalObject["r1"];
            var r2 = (JsNumber)GlobalObject["r2"];

            ((bool)r1).Should().BeTrue();
            ((int)r2).Should().Be(initial);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5.33)]
        [InlineData(120.6686668668)]
        [InlineData(double.MaxValue)]
        [InlineData(double.MinValue)]
        [InlineData(Math.PI)]
        [InlineData(Math.E)]
        [InlineData(double.NaN, true)]
        [InlineData(double.PositiveInfinity, true)]
        [InlineData(double.NegativeInfinity, true)]
        public void ConversionTheoryDouble(double initial, bool skipC1 = false)
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            var number = (JsNumber)initial;
            var GlobalObject = JsObject.GlobalObject;
            GlobalObject["num"] = number;
            GlobalObject["r1"] = JsValueRef.Null;
            GlobalObject["r2"] = JsValueRef.Null;

            if(skipC1)
                JsContext.RunScript($"r2 = num");
            else
                JsContext.RunScript($"r1 = (num == {initial}); r2 = num");

            var r1 = (JsBool)GlobalObject["r1"];
            var r2 = (JsNumber)GlobalObject["r2"];

            if(!skipC1)
                ((bool)r1).Should().BeTrue();
            ((double)r2).Should().Be(initial);
        }
    }
}
