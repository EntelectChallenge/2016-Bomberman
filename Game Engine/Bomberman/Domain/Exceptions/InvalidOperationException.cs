using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Exceptions
{
    public class InvalidOperationException : Exception
    {
        public InvalidOperationException(string message)
            : base(message)
        {
            
        }

    }
}
