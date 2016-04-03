using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Exceptions
{
    public class InvalidEntityTypeException : Exception
    {
        public InvalidEntityTypeException(String message)
            : base(message)
        {
            
        }
    }
}
