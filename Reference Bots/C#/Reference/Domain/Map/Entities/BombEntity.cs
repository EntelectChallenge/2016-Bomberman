using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reference.Domain.Map.Entities
{
    public class BombEntity : BaseEntity
    {
        public PlayerEntity Owner { get; set; }
        public int BombRadius { get; set; }
        public int BombTimer { get; set; }
        public bool IsExploding { get; set; }
        public int Points { get; set; }

        public override string ToString()
        {
            return String.Format("{0}(X:{1}, Y:{2}, Raduis:{3}, Timer:{4}, Owner:{5})", GetType().Name, Location.X, Location.Y, BombRadius, BombTimer, Owner.Key);
        }
    }
}
