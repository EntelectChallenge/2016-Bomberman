using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Interfaces;
using Newtonsoft.Json;

namespace Domain.Entities
{
    public class BombEntity : BaseEntity
    {
        public PlayerEntity Owner { get; set; }
        public int BombRadius { get; set; }
        public int BombTimer { get; set; }
        public bool IsExploding { get; set; }

        [JsonIgnore]
        public int Points { get; set; }

        public override char GetMapSymbol()
        {
            return Location.IsSameCoordinates(Owner.Location)
                ? Char.ToLower(Owner.GetMapSymbol())
                : Convert.ToChar(BombTimer.ToString());
        }

        public override bool IsDestructable()
        {
            return true;
        }

        public override string ToString()
        {
            return String.Format("{0}(X:{1}, Y:{2}, Radius:{3}, Timer:{4}, Owner:{5})", GetType().Name, Location.X, Location.Y, BombRadius, BombTimer, Owner.Key);
        }
    }
}
