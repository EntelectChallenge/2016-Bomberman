using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPowerUpEntity : IEntity
    {
        void PerformPowerUp(PlayerEntity playerEntity);
    }
}
