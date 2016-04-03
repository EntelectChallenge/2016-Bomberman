using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class DestructibleWallEntity : BaseEntity
    {
        public override char GetMapSymbol()
        {
            return '+';
        }

        public override bool IsDestructable()
        {
            return true;
        }

        public override string ToString()
        {
            return String.Format("{0}(X:{1}, Y:{2})", GetType().Name, Location.X, Location.Y);
        }
    }
}
