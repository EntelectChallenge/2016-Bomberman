using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reference.Domain.Map.Entities
{
    public class DestructibleWallEntity : BaseEntity
    {
        public override int GetPossiblePoints()
        {
            return 30;
        }

        public override string ToString()
        {
            return String.Format("{0}(X:{1}, Y:{2})", GetType().Name, Location.X, Location.Y);
        }
    }
}
