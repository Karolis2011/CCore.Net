using CCore.Net.JsRt;
using CCore.Net.Managed;
using CCore.Net.Runtimes;
using System;

Action<JsValue> print = (v) => Console.WriteLine(v.ConvertToString());

using var runtime = new BasicJsRuntime(CCore.Net.JsRuntimeAttributes.None);
using var scope = new BasicJsRuntime.Scope(runtime);

JsObject.GlobalObject["print"] = new JsManagedFunction(print);

JsContext.RunScript(@"
for(let i = 1; i <= 100; i++){
    if(i % 3 === 0 && i % 5 === 0){
        print(""FizzBuzz"");
    }else if (i % 3 === 0)
    {
        print(""Fizz"");
    }
    else if (i % 5 === 0)
    {
        print(""Buzz"");
    }
    else
    {
        print(i);
    }
}
");
