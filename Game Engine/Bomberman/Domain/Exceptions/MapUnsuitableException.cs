using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Exceptions
{
    public class MapUnsuitableException : Exception
    {
        public MapUnsuitableException()
            : base("Generated map was not suitable for the chosen power-up generation strategy.")
        {
            
        }

    }
}
