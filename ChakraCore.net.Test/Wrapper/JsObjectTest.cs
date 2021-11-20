using ChakraCore.Net.JsRt;
using ChakraCore.Net.Managed;
using Xunit;
using FluentAssertions;
using ChakraCore.Net.Test.Runtimes.Fixtures;
using ChakraCore.Net.Runtimes;

namespace ChakraCore.Net.Test.Wrapper
{
    public class JsObjectTest : IClassFixture<BasicJsRuntimeFixture>
    {
        BasicJsRuntimeFixture fixture;

        public JsObjectTest(BasicJsRuntimeFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Supports()
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            JsValueRef obj = JsValueRef.Invalid;
            obj = JsContext.RunScript("({ x: 5 })");
            JsObject.isSupported(obj.ValueType, obj).Should().BeTrue();
        }

        [Fact]
        public void Acessors()
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            var obj = JsContext.RunScript("({ x: 5, [5]: 15 })");

            JsObject.isSupported(obj.ValueType, obj).Should().BeTrue();

            var managedObj = JsValue.FromRaw(obj);
            managedObj.Should().BeOfType<JsObject>();
            var managedObject = (JsObject)managedObj;

            var x = (JsValue)managedObject["x"];
            x.Should().BeOfType<JsNumber>();
            ((int)(JsNumber)x).Should().Be(5);

            var five = (JsValue)managedObject[x];
            five.Should().BeOfType<JsNumber>();
            ((int)(JsNumber)five).Should().Be(15);
        }
    }
}
