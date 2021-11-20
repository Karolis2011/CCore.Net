using ChakraCore.Net.JsRt;
using ChakraCore.Net.Managed;
using ChakraCore.Net.Runtimes;
using ChakraCore.Net.Test.Runtimes.Fixtures;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ChakraCore.Net.Test.Wrapper
{
    public class JsArrayTest : IClassFixture<BasicJsRuntimeFixture>
    {
        BasicJsRuntimeFixture fixture;
        private readonly ITestOutputHelper outputHelper;

        public JsArrayTest(BasicJsRuntimeFixture fixture, ITestOutputHelper testOutputHelper)
        {
            this.fixture = fixture;
            outputHelper = testOutputHelper;
        }

        [Fact]
        public void Make()
        {
            using var a = new BasicJsRuntime.Scope(fixture.Runtime);

            var array = new JsArray(3);
            array.AddRef();

            array[0] = (JsNumber)5;
            array[1] = (JsString)"Hello world";
            array[2] = (JsString)"Test2";

            ((string)array.ConvertToString()).Should().Be("5,Hello world,Test2");
        }
    }
}