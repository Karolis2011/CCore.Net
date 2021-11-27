# CCore.Net a convenient wrapper around ChakraCore JavaScript engine.

This wrapper was made, because most of wrappers around CC seemed complicated to use, didn't provide any guard rails or just seemed like those wrappers were made for C, an not C#. This wrapper tries to use many .Net code features that allow it it be convenient and flexible.

# Parts of wrapper

Wrapper is made up of following layers and parts

## JsRt layer

This is the barebones layer that is often just convenient functions surrounding ChakraCore. In our case we have decided to leave it exposed, so you can use it however you want. If you want to achieve something that is not in managed layer, you can freely do so, just make sure to be careful and use it with caution, as it's improper use may cause crashes.

## Managed layer

This layer has two main components that are responsible for all the work.

### Runtimes

A runtime is a separate execution environment for you.

`BasicJsRuntime` is probably the simplest you can get. All it is is just a wrapper for `JsRt.JsContext` and `JsRt.JsRuntime` and simple list to track Managed objects that may need a cleanup.

`ScheduledJsRuntime` is an alias for `ScheduledJsRuntime<JsPFIFOScheduler>` what is a scheduled runtime. It has it's own scheduler, witch is responsible for executing jobs with underlying `JsRt.JsContext`. This runtime is useful when interactions needed to be done with runtime originate from multi-threaded environment. Default scheduler should suffice for most scenarios, as it works on prioritized first in first out algorithm.

### Managed objects

Those are additional wrapper around `JsValueRef`, providing convenient way to store references and determine when they need to be freed so they could be garbage collected.

A more exclusive are `JsNativeFunction` and `JsManagedFunction`, witch do more tracking when objects associated with them need to be freed.

#### Also there is `JsTypeMapper`

Well yes, this item was at most costs considered to be avoided, but there needed to be a central place to store Host -> Runtime and Runtime -> Host type conversions.

# Getting started

Obtain yourself CCCore.Net managed .net library by downloading this git repository and building it yourself or downloading one from releases section. You also will need ChakraCore native library.

So when you have set up your libraries. You can just create a new Runtime of you flavor and set it as your active one.

```cs
using var runtime = new BasicJsRuntime(CCore.Net.JsRuntimeAttributes.None);
using var _ = new BasicJsRuntime.Scope(runtime);
```

Then you can ask it to run script:

```cs
JsContext.RunScript(@"function hello() {
    const World = { depression: true, love: false }

    return 'Hello world'
}");
```

And then you can get values from js...

```cs
var value = (JsFunction)JsContext.RunScript("hello");
```

And work with them...

```cs
value.Invoke(JsObject.GlobalObject)
Console.WriteLine(value.ConvertToString())
```

# Inspiration

I want to mention following projects who have inspired or have somehow contributed to this wrapper.

- https://github.com/Taritsyn/JavaScriptEngineSwitcher - Great obfuscation of engine details to allow just basic things. TypeMapper was great influence on some aspects.
-
