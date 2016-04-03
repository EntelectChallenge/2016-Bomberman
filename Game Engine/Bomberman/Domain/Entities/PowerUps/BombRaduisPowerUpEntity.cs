using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Interfaces;

namespace Domain.Entities.PowerUps
{
    public class BombRaduisPowerUpEntity : BaseEntity , IPowerUpEntity
    {
        public override char GetMapSymbol()
        {
            return '!';
        }

        public override bool IsDestructable()
        {
            return false;
        }

        public void PerformPowerUp(PlayerEntity playerEntity)
        {
            playerEntity.BombRadius *= 2;
        }

        public override bool StopsBombBlast()
        {
            return false;
        }

        public override string ToString()
        {
            return String.Format("{0}(X:{1}, Y:{2})", GetType().Name, Location.X, Location.Y);
        }
    }
}
