using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.Net.JsRt
{
    public class JsTerminationException : JsException
    {
        public JsTerminationException(JsErrorCode code) : base(code, "Script was terminated") { }
        public JsTerminationException(JsErrorCode code, string message) : base(code, message) { }
    }
}
