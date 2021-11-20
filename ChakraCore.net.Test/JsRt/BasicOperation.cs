using ChakraCore.Net.JsRt;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace ChakraCore.Net.Test.JsRt
{
    public class BasicOperation : IClassFixture<EngineFixture>
    {
        EngineFixture fixture;

        public BasicOperation(EngineFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void ScriptExecution()
        {
            using (new JsContext.Scope(fixture.context))
            {
                var ow = JsContext.RunScript("x = 5");
                var glob = JsValueRef.GlobalObject;
                var value = glob.GetProperty("x");
                Assert.Equal(ow, value);
                Assert.Equal(JsValueType.Number, value.ValueType);
                Assert.Equal(5, value.ToInt32());
            }
        }

        [Fact]
        public void JsTermination()
        {
            using (new JsContext.Scope(fixture.context))
            {
                using (var timer = new Timer(state => fixture.runtime.Disabled = true))
                {
                    timer.Change(100, Timeout.Infinite);
                    try
                    {
                        JsContext.RunScript("while(true){}");
                    }
                    catch (JsScriptException ex)
                    {
                        if (ex.ErrorCode != JsErrorCode.ScriptTerminated)
                            throw;
                    }
                }
                fixture.runtime.Disabled = false;
            }
        }
    }
}
