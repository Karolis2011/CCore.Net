using CCore.Net;
using CCore.Net.JsRt;
using System;
using Xunit;
using FluentAssertions;
using CCore.Net.Managed;
using CCore.Net.Test.Runtimes.Fixtures;
using CCore.Net.Runtimes;

namespace CCore.Net.Test.Wrapper
{
    public class JsValueTest : IClassFixture<BasicJsRuntimeFixture>
    {
        BasicJsRuntimeFixture fixture;

        public JsValueTest(BasicJsRuntimeFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Convert()
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            var five = JsContext.RunScript("5");
            var num = JsValue.FromRaw(five);

            five.ValueType.Should().Be(JsValueType.Number);
            five.ToInt32().Should().Be(5);

            num.Should().BeOfType<JsNumber>();
            ((int)(JsNumber)num).Should().Be(5);
        }

        [Fact]
        public void Supports()
        {
            JsValue.isSupported(JsValueType.Undefined, JsValueRef.Invalid).Should().BeFalse();
        }

        public WeakReference GCTestPart(JsValueRef value)
        {
            var managed = new JsString(value);
            managed.AddRef();
            return new WeakReference(managed);
        }

        [Fact]
        public void GCTest()
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            uint refs = 0, finalRefs = 0;
            JsValueRef five = JsValueRef.Invalid;
            WeakReference reference = null;

            five = JsContext.RunScript("'AAAA'");

            reference = GCTestPart(five);
            refs = five.AddRef();

            Action dispose = () =>
            {
                if (reference.IsAlive)
                    if (reference.Target is IDisposable disposable)
                        disposable.Dispose();
            };

            dispose.Should().NotThrow();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            finalRefs = five.Release();

            reference.IsAlive.Should().BeFalse();

            refs.Should().Be(2);
            finalRefs.Should().Be(0);
        }


    }
}
