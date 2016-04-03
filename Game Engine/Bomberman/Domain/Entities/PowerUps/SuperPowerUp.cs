using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Common;
using Domain.Interfaces;

namespace Domain.Entities.PowerUps
{
    public class SuperPowerUp : BaseEntity, IPowerUpEntity
    {
        private readonly int _bonusPoints;

        public SuperPowerUp(int bonusPoints)
        {
            _bonusPoints = bonusPoints;
        }

        public override char GetMapSymbol()
        {
            return '$';
        }

        public override bool IsDestructable()
        {
            return false;
        }

        public void PerformPowerUp(PlayerEntity playerEntity)
        {
            new BombBagPowerUpEntity().PerformPowerUp(playerEntity);
            new BombRaduisPowerUpEntity().PerformPowerUp(playerEntity);

            playerEntity.AddPoints(_bonusPoints);
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
