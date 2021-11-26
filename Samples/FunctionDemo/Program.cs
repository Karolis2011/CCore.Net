using CCore.Net.JsRt;
using CCore.Net.Managed;
using CCore.Net.Runtimes;
using System;


using var runtime = new BasicJsRuntime(CCore.Net.JsRuntimeAttributes.None);
using var scope = new BasicJsRuntime.Scope(runtime);

var value = JsContext.RunScript(@"(function () {})");
value.CallFunction(JsObject.GlobalObject);
var managedValue = new JsFunction(value);
managedValue.Invoke(JsObject.GlobalObject);
Console.WriteLine(managedValue.ConvertToString());
