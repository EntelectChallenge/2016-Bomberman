using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reference.Domain.Map.Entities.PowerUps
{
    public class SuperPowerUp : BaseEntity, IPowerUp
    {
        public override int GetPossiblePoints()
        {
            return 100;
        }

        public override string ToString()
        {
            return String.Format("{0}(X:{1}, Y:{2})", GetType().Name, Location.X, Location.Y);
        }
    }
}
