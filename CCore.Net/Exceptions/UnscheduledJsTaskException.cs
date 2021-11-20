using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.Net.Exceptions
{
    public class UnscheduledJsTaskException : InvalidJsTaskStateException
    {
        public UnscheduledJsTaskException() : base("JsTask is unscheduled, meaning it won't be executed. Make sure to call Start() method before this method.")
        {
        }

        public UnscheduledJsTaskException(string message) : base(message)
        {
        }
    }
}
