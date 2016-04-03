using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Exceptions
{
    public class LocationOutOfBoundsException : Exception
    {
        public LocationOutOfBoundsException(String message) :
            base(message)
        {
            
        }
    }
}
