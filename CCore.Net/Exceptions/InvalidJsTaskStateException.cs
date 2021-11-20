using System;
using System.Collections.Generic;
using System.Text;

namespace CCore.Net.Exceptions
{
    public class InvalidJsTaskStateException : Exception
    {
        public InvalidJsTaskStateException(string message) : base(message)
        {
        }
    }
}
