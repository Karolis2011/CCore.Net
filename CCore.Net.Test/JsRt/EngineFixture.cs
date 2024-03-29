﻿using CCore.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.Net.JsRt
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "<Pending>")]
    public class EngineFixture : IDisposable
    {
        public JsRuntime runtime;
        public JsContext context;

        public EngineFixture()
        {
            runtime = JsRuntime.Create(JsRuntimeAttributes.AllowScriptInterrupt);
            context = runtime.CreateContext();
            context.AddRef();
        }

        public void Dispose()
        {
            context.Release();
            runtime.Dispose();
        }
    }
}
