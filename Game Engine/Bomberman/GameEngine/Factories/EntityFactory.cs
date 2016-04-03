using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Entities;
using Domain.Entities.PowerUps;
using Domain.Interfaces;
using GameEngine.Exceptions;
using GameEngine.Properties;

namespace GameEngine.Factories
{
    public class EntityFactory
    {
        public IEntity ConstructEntity(EntityType entityType)
        {
            switch (entityType)
            {
                    case EntityType.Player:
                    return new PlayerEntity();
                    case EntityType.IndesctructibleWall:
                    return new IndestructibleWallEntity();
                    case EntityType.DesctructibleWall:
                    return new DestructibleWallEntity();
                    case EntityType.Bomb:
                    return new BombEntity();
                    case EntityType.BombBagPowerUp:
                    return new BombBagPowerUpEntity();
                    case EntityType.BombRaduisPowerUp:
                    return new BombRaduisPowerUpEntity();
                    case EntityType.SuperPowerUp:
                    return new SuperPowerUp(Settings.Default.SuperPowerUpPoints);
                default:
                    throw new InvalidEntityTypeException("Factory cannot create entity of type " + entityType);
            }
        }

        public enum EntityType
        {
            Player,
            IndesctructibleWall,
            DesctructibleWall,
            Bomb,
            BombBagPowerUp,
            BombRaduisPowerUp,
            SuperPowerUp
        }
    }
}
