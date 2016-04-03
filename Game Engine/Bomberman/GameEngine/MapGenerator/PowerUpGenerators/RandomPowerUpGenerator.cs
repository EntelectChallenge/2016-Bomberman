using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using GameEngine.Factories;
using GameEngine.Properties;

namespace GameEngine.MapGenerator.PowerUpGenerators
{
    public class RandomPowerUpGenerator : IPowerUpGenerator 
    {
        private readonly EntityFactory _entityFactory;
        public Random Rand { get; set; }

        public RandomPowerUpGenerator(EntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
            Rand = new Random();
        }

        public void GeneratePowerUps(GameMap gameMap)
        {
            var bombagRandom = Rand;
            var bombRadiiRandom = Rand;

            var quadrantWidth = (gameMap.MapWidth) / 2 + 1;
            var quadrantHeight = (gameMap.MapWidth) / 2 + 1;

            const int quadrants = 4;
            var translator = new CoordinateTranslator(gameMap.MapWidth + 1, gameMap.MapHeight + 1);

            for (var i = 0; i < quadrants; i++)
            {
                var bomBags = (Settings.Default.BombBagPowerUpMultiplier * gameMap.RegisteredPlayerEntities.Count) / quadrants;
                var bomRadii = (Settings.Default.BombRaduisPowerUpMultiplier * gameMap.RegisteredPlayerEntities.Count) / quadrants;

                while (bomBags > 0)
                {
                    var x = bombagRandom.Next(1, quadrantWidth);
                    var y = bombagRandom.Next(1, quadrantHeight);

                    x = x == 0 ? 1 : x;
                    y = y == 0 ? 1 : y;

                    var location = ToQuadrantLocation(translator, 4 - i, x, y);

                    var gameBlock = gameMap.GetBlockAtLocation(location.X, location.Y);
                    if (gameBlock.PowerUpEntity == null && gameBlock.Entity != null && gameBlock.Entity.GetType() == typeof(DestructibleWallEntity))
                    {
                        gameBlock.SetPowerUpEntity((IPowerUpEntity)_entityFactory.ConstructEntity(EntityFactory.EntityType.BombBagPowerUp));
                        bomBags--;
                    }

                }

                while (bomRadii > 0)
                {
                    var x = bombRadiiRandom.Next(1, quadrantWidth);
                    var y = bombRadiiRandom.Next(1, quadrantHeight);

                    var location = ToQuadrantLocation(translator, i, x, y);

                    var gameBlock = gameMap.GetBlockAtLocation(location.X, location.Y);
                    if (gameBlock.PowerUpEntity == null && gameBlock.Entity != null && gameBlock.Entity.GetType() == typeof(DestructibleWallEntity))
                    {
                        gameBlock.SetPowerUpEntity((IPowerUpEntity)_entityFactory.ConstructEntity(EntityFactory.EntityType.BombRaduisPowerUp));
                        bomRadii--;
                    }
                }
            }

            var block = gameMap.GetBlockAtLocation(gameMap.MapWidth / 2 + 1, gameMap.MapHeight / 2 + 1);
            block.SetEntity(null);
            block.SetPowerUpEntity((IPowerUpEntity)_entityFactory.ConstructEntity(EntityFactory.EntityType.SuperPowerUp));
        }

        public bool IsMapSuitable(GameMap map)
        {
            return true;
        }

        private Location ToQuadrantLocation(CoordinateTranslator translator, int quadrant, int x, int y)
        {
            switch (quadrant)
            {
                case 1:
                    return new Location(translator.TranslateX(x), y);
                case 2:
                    return new Location(x, translator.TranslateY(y));
                case 3:
                    return new Location(translator.TranslateX(x), translator.TranslateY(y));
                default:
                    return new Location(x, y);
            }
        }
    }
}
