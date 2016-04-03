using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Common;
using Domain.Interfaces;
using Newtonsoft.Json;

namespace Domain.Entities
{
    public abstract class BaseEntity : IEntity
    {
        public Location Location { get; set; }

        public abstract char GetMapSymbol();
        public abstract bool IsDestructable();

        public virtual bool StopsBombBlast()
        {
            return true;
        }

        public override string ToString()
        {
            return String.Format("{0}(X:{1}, Y:{2})", GetType().Name, Location.X, Location.Y);
        }
    }
}
