
using CCore.Net.JsRt;
using CCore.Net.Managed;
using CCore.Net.Runtimes;

Action<JsValue> print = (v) => Console.WriteLine(v.ConvertToString());

using var runtime = new BasicJsRuntime(CCore.Net.JsRuntimeAttributes.None);
using var scope = new BasicJsRuntime.Scope(runtime);

JsObject.GlobalObject["p"] = JsManagedFunction.Obtain(print);;
var libModule = JsModule.NewModule("lib.dll", @"
export const print = p;
");
JsObject.GlobalObject["print"] = JsUndefined.Undefined;

var rootModule = JsModule.NewModule("root.js");
rootModule.SetNotifyModuleReadyCallback((module, error) =>
{
    Console.WriteLine($"Module {module.GetUrl()} is ready");
    var obj = module.Evaluate();
    Console.WriteLine(obj.ConvertToString().ToString());
    if (error.IsValid)
    {
        Console.WriteLine($"Error: {error.ConvertToString()}");
    }
    return JsErrorCode.NoError;
});
rootModule.SetFetchImportedModuleCallback((JsModule loadingModule, JsValueRef specifier, out JsModule found) =>
{
    var specifierStr = specifier.ToString();
    Console.WriteLine($"Loading module {specifierStr}");
    if (specifierStr == "lib.dll" || specifierStr == "lib")
    {
        found = libModule;
        return JsErrorCode.NoError;
    }
    found = JsModule.NewModule(specifierStr);
    return JsErrorCode.NoError;
});

rootModule.Parse(@"
p(print);
import { print } from 'lib';
p(print);
export const a = 1;
print('Hi');
export default { a };
", JsSourceContext.None);
Console.WriteLine("Parsed");
