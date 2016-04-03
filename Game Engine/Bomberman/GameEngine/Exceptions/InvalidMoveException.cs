using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Exceptions
{
    public class InvalidMoveException : Exception
    {
        public InvalidMoveException(String reason) :
            base(reason)
        {
            
        }
    }
}
