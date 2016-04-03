using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reference.Domain.Map.Entities
{
    public class PlayerEntity : BaseEntity
    {
        public String Name { get; set; }
        public char Key { get; set; }
        public int Points { get; set; }
        public bool Killed { get; set; }
        public int BombBag { get; set; }
        public int BombRadius { get; set; }

        public override int GetPossiblePoints()
        {
            return 100;
        }

        public override string ToString()
        {
            return String.Format("{0}(X:{1}, Y:{2}, Key:{3})", GetType().Name, Location.X, Location.Y, Key);
        }
    }
}
