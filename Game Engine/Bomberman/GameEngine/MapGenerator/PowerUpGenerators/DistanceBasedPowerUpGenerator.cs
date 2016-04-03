using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Common;
using Domain.Interfaces;
using GameEngine.Factories;
using Domain.Entities;
using GameEngine.Loggers;

namespace GameEngine.MapGenerator.PowerUpGenerators {

    // Attempts to generate power-ups fairly by placing each of them the same distance away from each player:
    // So if player 1's quadrant has power ups that are 4,6,7 and 9 moves away, each of the other players will
    // also have power ups at the same distances away from them.
    //
    // The order is also controlled for fairness, so if player 1 has a bomb-bag as their nearest power-up the
    // other players will also have a bomb-bag nearest.
    //
    // This code assumes a 4 player game.
    class DistanceBasedPowerUpGenerator : IPowerUpGenerator {

        private readonly int NUM_POWERUPS_PER_PLAYER = 6;
        private readonly EntityFactory _entityFactory;
        private ILogger _logger = new ConsoleLogger();
        public Random Rand { get; set; }

        public DistanceBasedPowerUpGenerator(EntityFactory entityFactory) {
            _entityFactory = entityFactory;
            Rand = new Random();
        }

        public void GeneratePowerUps(GameMap map) {
            var powerUpOrder = PlanPowerUpOrder();
            var powerUpDistances = PlanPowerUpDistribution(map, powerUpOrder);

            const int quadrants = 4;
            var translator = new CoordinateTranslator(map.MapWidth + 1, map.MapHeight + 1);
            GameBlock block;

            for (var quadrant = 0; quadrant < quadrants; quadrant++) {
                for (var powerUpCount = 0; powerUpCount < powerUpOrder.Count; powerUpCount++) {
                    block = GetRandomBlockFromQuadrantAtDistance(map, quadrant, translator, powerUpDistances[powerUpCount]);
                    _logger.LogDebug("Quadrant : " + quadrant + "; Distance: " + powerUpDistances[powerUpCount] + "; Block: " + (block == null ? "NULL" : block.Location.ToString()));
                    var powerUp = powerUpOrder[powerUpCount];
                    if (powerUp == '&') {
                        block.SetPowerUpEntity((IPowerUpEntity)_entityFactory.ConstructEntity(EntityFactory.EntityType.BombBagPowerUp));
                    } else if (powerUp == '!') {
                        block.SetPowerUpEntity((IPowerUpEntity)_entityFactory.ConstructEntity(EntityFactory.EntityType.BombRaduisPowerUp));
                    }
                }
            }

            block = map.GetBlockAtLocation(map.MapWidth / 2 + 1, map.MapHeight / 2 + 1);
            block.SetEntity(null);
            block.SetPowerUpEntity((IPowerUpEntity)_entityFactory.ConstructEntity(EntityFactory.EntityType.SuperPowerUp));
        }

        public bool IsMapSuitable(GameMap map) {
            Dictionary<int, int> blocksPerDistance = CountBlocksPerDistance(map);

            int count = 0;
            foreach (int distance in blocksPerDistance.Keys) {
                if (blocksPerDistance[distance] > 1) {
                    count++;
                }
            }

            return count > NUM_POWERUPS_PER_PLAYER;
        }

        private GameBlock GetRandomBlockFromQuadrantAtDistance(GameMap gameMap, int quadrant, CoordinateTranslator translator, int distance) {
            var quadrantWidth = (gameMap.MapWidth) / 2;
            var quadrantHeight = (gameMap.MapWidth) / 2;
            var playerLocation = ToQuadrantLocation(translator, quadrant, 1, 1);
            _logger.LogDebug("Quadrant: " + quadrant + "; PlayerLocation: " + playerLocation.ToString());

            var blocks = new List<GameBlock>();
            for (var x = 1; x < quadrantWidth; x++) {
                for (var y = 1; y < quadrantHeight; y++) {
                    int radius = (int) Math.Floor(DetermineRadius(1, 1, x, y));
                    if (radius != distance) {
                        continue;
                    }

                    var location = ToQuadrantLocation(translator, quadrant, x, y);
                    GameBlock block = gameMap.GetBlockAtLocation(location.X, location.Y);
                    if (block.Entity != null && block.Entity.GetType() == typeof(DestructibleWallEntity)) {
                        blocks.Add(block);
                        _logger.LogDebug("X: " + location.X + "; Y: " + location.Y + "; radius: " + radius);
                    }
                }
            }

            if (blocks.Count == 0) {
                return null;
            }

            var result = blocks[Rand.Next(blocks.Count)];
            blocks.Clear();
            return result;
        }

        private List<int> PlanPowerUpDistribution(GameMap gameMap, List<char> powerUpOrder) {
            var blocksPerDistance = CountBlocksPerDistance(gameMap);

            // Add all distances that have multiple blocks - those are ideal so players can't predict their powerup
            // locations based on what their opponents find...
            var plan = new List<int>();
            var allDistances = new List<int>(blocksPerDistance.Keys);
            var distancesWithOneBlock = new List<int>();
            allDistances.Sort();
            foreach (int distance in allDistances) {
                if (blocksPerDistance[distance] > 1) {
                    plan.Add(distance);
                } else if (distance > 5) {
                    distancesWithOneBlock.Add(distance);
                }
            }

            // Guarantee at least 1 powerup within 5 squares of the player's starting position
            if (plan.Count > 0 && plan[0] > 5) {
                foreach (int distance in allDistances) {
                    if (distance > 2) { // Needs to be close, but outside the safe distance
                        plan.Insert(0, distance);
                        break;
                    }
                }
            }

            // Narrow down the list of distance bands if it's too large...
            while (plan.Count > 6) {
                int removeAt = Rand.Next(1, 4);
                plan.RemoveAt(removeAt);
            }
            
            plan.Sort();
            return plan;
        }

        private Dictionary<int, int> CountBlocksPerDistance(GameMap gameMap) {
            var quadrantWidth = (gameMap.MapWidth) / 2;
            var quadrantHeight = (gameMap.MapWidth) / 2;

            var blocksPerDistance = new Dictionary<int, int>();
            for (var x = 1; x < quadrantWidth; x++) {
                for (var y = 1; y < quadrantHeight; y++) {
                    GameBlock block = gameMap.GetBlockAtLocation(x, y);
                    if (block.Entity != null && block.Entity.GetType() == typeof(DestructibleWallEntity)) {
                        int radius = (int) Math.Floor(DetermineRadius(1, 1, x, y));

                        if (blocksPerDistance.ContainsKey(radius)) {
                            blocksPerDistance[radius]++;
                        } else {
                            blocksPerDistance.Add(radius, 1);
                        }
                    }
                }
            }

            return blocksPerDistance;
        }

        // Generates one of the following power-up orders:
        //   & ! ! & ! !
        //   ! & ! ! & !
        //   ! ! & ! ! &
        private List<char> PlanPowerUpOrder() {
            var plan = new List<char>();

            int typeCount = Rand.Next(0, 3);
            for (int i = 0; i < NUM_POWERUPS_PER_PLAYER; i++) {
                if ((typeCount + i) % 3 == 0) {
                    plan.Add('&');
                } else {
                    plan.Add('!');
                }
            }

            return plan;
        }

        private double DetermineRadius(int centreX, int centreY, int x, int y) {
            var pointX = centreX - x;
            var pointY = centreY - y;

            if (pointX < 0) {
                pointX = x - centreX;
            }

            if (pointY < 0) {
                pointY = y - centreY;
            }

            //return Math.Sqrt(pointX * pointX + pointY * pointY);
            // Instead of using z = sqrt(x*x + y*y), use the straight line distance - should be more fair
            return Math.Abs(pointX) + Math.Abs(pointY);
        }

        private Location ToQuadrantLocation(CoordinateTranslator translator, int quadrant, int x, int y) {
            switch (quadrant) {
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
