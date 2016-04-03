using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestHarness.Exceptions
{
    public class TimeLimitExceededException : Exception
    {
        public TimeLimitExceededException(string message) :
            base(message)
        {
            
        }
    }
}
