using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reference.Domain.Map.Entities
{
    public abstract class BaseEntity : IEntity
    {
        public Location Location { get; set; }

        public virtual int GetPossiblePoints()
        {
            return 0;
        }

        public override string ToString()
        {
            return String.Format("{0}(X:{1}, Y:{2})", GetType().Name, Location.X, Location.Y);
        }
    }
}
