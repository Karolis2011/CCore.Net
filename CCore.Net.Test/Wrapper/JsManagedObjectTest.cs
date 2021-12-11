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
    public class JsManagedObjectTest : IClassFixture<BasicJsRuntimeFixture>
    {
        BasicJsRuntimeFixture fixture;
        public JsManagedObjectTest(BasicJsRuntimeFixture fixture)
        {
            this.fixture = fixture;
        }

        public class Cls
        {
            public int Counter { get; private set; } = 5;
            public int Other { get; set; }
            public string field = "aa";
            public bool flag = false;

            public void set(int counter) => Counter = counter;
            public void set(int counter, string field)
            {
                Counter = counter;
                this.field = field;
            }

            public void trigger() => flag = true;

            public JsString toString() => $"{field}@{Other}@{Counter}";
        }

        [Fact]
        public void TestProperties()
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            var obj = new Cls();
            var mObj = new JsManagedObject(obj);

            JsObject.GlobalObject["cls"] = mObj;

            var result = (JsNumber)JsContext.RunScript("cls.Counter");
            ((int)result).Should().Be(obj.Counter);

            JsContext.RunScript("cls.Counter = 10");
            obj.Counter.Should().Be(result);

            obj.Other.Should().Be(default);
            JsContext.RunScript("cls.Other = 27");
            obj.Other.Should().Be(27);
        }

        [Fact]
        public void TestFields()
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            var obj = new Cls();
            var mObj = new JsManagedObject(obj);

            JsObject.GlobalObject["cls"] = mObj;

            var result = (JsString)JsContext.RunScript("cls.field");
            ((string)result).Should().Be("aa");

            JsContext.RunScript("cls.field = `i${cls.Other}`");
            obj.field.Should().Be($"i{obj.Other}");

            var flagResult = (JsBool)JsContext.RunScript("cls.flag");
            ((bool)flagResult).Should().Be(obj.flag);
        }

        [Fact]
        public void TestMethods()
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            var obj = new Cls();
            var mObj = new JsManagedObject(obj);

            JsObject.GlobalObject["cls"] = mObj;

            obj.Counter.Should().Be(5);
            JsContext.RunScript("cls.set(9)");
            obj.Counter.Should().Be(9);
            obj.field.Should().Be("aa");
            JsContext.RunScript("cls.set(3, 'bb')");
            obj.Counter.Should().Be(3);
            obj.field.Should().Be("bb");

            obj.flag.Should().BeFalse();
            JsContext.RunScript("cls.trigger()");
            obj.flag.Should().BeTrue();


            var result = (JsValue)JsContext.RunScript("cls");
            var stringRepr = result.ConvertToString();
            ((string)stringRepr).Should().Be("bb@0@3");
        }

        [Fact]
        public void GarbageCollection()
        {
            using var s = new BasicJsRuntime.Scope(fixture.Runtime);

            var obj = new Cls();
            var mObj = new JsManagedObject(obj);

            fixture.Runtime.InternalRuntime.CollectGarbage();
            fixture.Runtime.InternalRuntime.CollectGarbage();

            mObj.IsFreeed.Should().BeTrue();
        }
    }
}
