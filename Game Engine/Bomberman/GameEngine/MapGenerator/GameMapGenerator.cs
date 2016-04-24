using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using GameEngine.Common;
using GameEngine.Factories;
using GameEngine.Properties;
using Domain.Exceptions;
using GameEngine.MapGenerator.PowerUpGenerators;

namespace GameEngine.MapGenerator
{
    public class GameMapGenerator
    {
        private readonly List<Player> _players;
        private readonly EntityFactory _entityFactory;
        private Random _rng;
        private IPowerUpGenerator _powerUpGenerator;
        public MapSizes MapSize { get; set; }

        public GameMapGenerator(List<Player> players)
        {
            if (players.Count <= 1 || players.Count > 12)
                throw new ArgumentException("Number of players should be between 2 and 12!");

            _players = players;
            
            _entityFactory = new EntityFactory();
            
            if (_players.Count == 4)
            {
                _powerUpGenerator = new DistanceBasedPowerUpGenerator(_entityFactory);
            }
            else
            {
                _powerUpGenerator = new RandomPowerUpGenerator(_entityFactory);
            }

            if (_players.Count <= 4)
                MapSize = MapSizes.Small;
            else if (_players.Count <= 8)
                MapSize = MapSizes.Meduim;
            else
                MapSize = MapSizes.Large;
        }

        public GameMapGenerator(List<Player> players, bool useRandomPowerupGenerator)
            :this(players)
        {
            if (useRandomPowerupGenerator)
            {
                _powerUpGenerator = new RandomPowerUpGenerator(_entityFactory);
            }
            else
            {
                _powerUpGenerator = new DistanceBasedPowerUpGenerator(_entityFactory);
            }
        }

        public GameMap GenerateGameMap(int seedSeed)
        {
            var seedGenerator = new Random(seedSeed);
            for (int randomSeed = seedGenerator.Next(); ; randomSeed = seedGenerator.Next())
            {
                try
                {
                    _rng = new Random(randomSeed);
                    _powerUpGenerator.Rand = _rng;
                    var mapSize = GetMapSize();
                    var gameMap = new GameMap(mapSize, mapSize, seedSeed);
                    GenerateIndestructableWalls(gameMap);
                    GeneratePlayers(gameMap);
                    GenerateDestructableWalls(gameMap);
                    GeneratePowerUps(gameMap);

                    return gameMap;
                }
                catch (MapUnsuitableException) { }
            }
        }

        private int GetMapSize()
        {
            switch (MapSize)
            {
                case MapSizes.Small:
                    return Settings.Default.SmallMapSize;
                case MapSizes.Meduim:
                    return Settings.Default.MeduimMapSize;
                default:
                    return Settings.Default.LargeMapSize;
            }
        }

        private void GenerateIndestructableWalls(GameMap gameMap)
        {
            //Generate Map Edges and created an indesctructable wall at every odd location
            using (var enumerator = gameMap.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var block = enumerator.Current;

                    if (block == null || block.Entity != null)
                        continue;

                    var x = block.Location.X;
                    var y = block.Location.Y;
                    if (x == 1 || y == 1 || x == gameMap.MapWidth || y == gameMap.MapHeight)
                    {
                        block.SetEntity(_entityFactory.ConstructEntity(EntityFactory.EntityType.IndesctructibleWall));
                    }
                    else if (block.Location.IsOdd())
                    {
                        block.SetEntity(_entityFactory.ConstructEntity(EntityFactory.EntityType.IndesctructibleWall));
                    }
                }
            }
        }

        private void GeneratePlayers(GameMap gameMap)
        {
            //Always place players in map corners
            for (var p = 0; p < _players.Count; p++)
            {
                var entity = new PlayerEntity()
                {
                    Name = _players[p].Name,
                    Key = IntToLetter(p),
                    BombBag = Settings.Default.DefaultBombBagSize,
                    BombRadius = Settings.Default.DefaultBomRaduis
                };

                PlacePlayerOnMap(entity, gameMap, p + 1);
                _players[p].PlayerRegistered(entity);
            }
        }

        private char IntToLetter(int value)
        {
            if (value >= 0)
            {
                return (char)('A' + value % 26);
            }
            return ' ';
        }

        private void PlacePlayerOnMap(PlayerEntity entity, GameMap gameMap, int playerNumber)
        {
            var totalPlayers = _players.Count;
            var playersPerSide = (int)Math.Ceiling(totalPlayers/4d);
            var playerSide = DeterminePlayerSide(playerNumber);
            var playerPosition = (int)Math.Ceiling(playerNumber / 4d);
            var payersPerWidth = (gameMap.MapWidth/playersPerSide);
            var palyersPerHeight = (gameMap.MapHeight/playersPerSide);


            var playerX = 0;
            var playerY = 0;

            switch (playerSide)
            {
                case 1: //Top
                    playerY = 2;
                    playerX = payersPerWidth * playerPosition - payersPerWidth + 2;
                    break;
                case 2://Bottom
                    playerY = gameMap.MapHeight - 1;
                    playerX = (gameMap.MapWidth + payersPerWidth) - (payersPerWidth * playerPosition) - 1;
                    break;
                case 3://Right
                    playerY = (gameMap.MapHeight + palyersPerHeight) - palyersPerHeight * playerPosition - 1;
                    playerX = 2;
                    break;
                default://Left
                    playerY = palyersPerHeight * playerPosition - palyersPerHeight + 2;
                    playerX = gameMap.MapWidth - 1;
                    break;
            }
            gameMap.RegisterPlayer(entity, playerX, playerY);
        }

        private int DeterminePlayerSide(int playerNumber)
        {
            var div = playerNumber/4d;
            var truncate = Math.Truncate(div);
            var percentage = div - truncate;

            if (percentage < 0.1)
            {
            return 4;
            }
            if (percentage < 0.26)
            {
                return 1;
            }
            if (percentage < 0.51)
            {
                return 2;
            }
            if (percentage < 0.76)
            {
                return 3;
            }
            return 4;
        }

        private void GenerateDestructableWalls(GameMap gameMap)
        {
            //Increase the quadrant size slightly so we can user the centre of the map is populated correctly
            var width = (gameMap.MapWidth)/2 + 2;
            var height = (gameMap.MapHeight)/2 + 2;

            var translator = new CoordinateTranslator(gameMap.MapHeight + 1, gameMap.MapWidth + 1);

            //We will generate the walls for one quadrant, and then copy that onto all the others to improve fairness
            for (var x = 1; x < width; x++)
            {
                for (var y = 1; y < height; y++)
                {
                    var rNumber = _rng.Next(0, 100);

                    GenerateDesctructableWall(gameMap, rNumber, x, y);
                    GenerateDesctructableWall(gameMap, rNumber, translator.TranslateX(x), y);
                    GenerateDesctructableWall(gameMap, rNumber, x, translator.TranslateY(y));
                    GenerateDesctructableWall(gameMap, rNumber, translator.TranslateX(x), translator.TranslateY(y));
                }
            }

            for (var x = width - 2; x < width + 2; x++)
            {
                for (var y = height - 2; y < height + 2; y++)
                {

                    GenerateDesctructableWall(gameMap, 0, x, y);
                    GenerateDesctructableWall(gameMap, 0, translator.TranslateX(x), y);
                    GenerateDesctructableWall(gameMap, 0, x, translator.TranslateY(y));
                    GenerateDesctructableWall(gameMap, 0, translator.TranslateX(x), translator.TranslateY(y));
                }
            }
        }

        private void GenerateDesctructableWall(GameMap gameMap, int randomNumber, int x, int y)
        {
            var frequency = Settings.Default.DesctructableWallFrequency * 100;
            var gameBlock = gameMap.GetBlockAtLocation(x, y);

            if (gameBlock.Entity != null && gameBlock.Entity.GetType() == typeof(PlayerEntity))
                GenerateWallAroundPlayer(gameMap, gameBlock.Entity);

            if(gameBlock.Entity != null || IsWithinPlayerSafeZone(gameMap, x, y))
                return;

            if (randomNumber < frequency)
            {
                gameBlock.SetEntity(_entityFactory.ConstructEntity(EntityFactory.EntityType.DesctructibleWall));
            }
        }

        private bool IsWithinPlayerSafeZone(GameMap gameMap, int x, int y)
        {
            const int safeZoneSize = 2;
            return gameMap.RegisteredPlayerEntities.Select(player => player.Location).Any(location => DetermineRaduis(location.X, location.Y, x, y) < safeZoneSize);
        }

        private void GenerateWallAroundPlayer(GameMap gameMap, IEntity playerEntity)
        {
            var location = playerEntity.Location;

            if (location.Y == 2 || location.Y == gameMap.MapHeight - 1)
            {
                var y = location.Y;
                var x = location.X + 3 < gameMap.MapWidth ? location.X + 3 : location.X - 3;

                var gameBlock = gameMap.GetBlockAtLocation(x, y);
                if (gameBlock.Entity == null)
                {
                    gameBlock.SetEntity(_entityFactory.ConstructEntity(EntityFactory.EntityType.DesctructibleWall));
                }
            }

            if (location.X == 2 || location.X == gameMap.MapWidth - 1)
            {
                var x = location.X;
                var y = location.Y + 3 < gameMap.MapHeight ? location.Y + 3 : location.Y - 3;

                var gameBlock = gameMap.GetBlockAtLocation(x, y);
                if (gameBlock.Entity == null)
                {
                    gameBlock.SetEntity(_entityFactory.ConstructEntity(EntityFactory.EntityType.DesctructibleWall));
                }
            }
        }

        private void GeneratePowerUps(GameMap gameMap)
        {
            if (!_powerUpGenerator.IsMapSuitable(gameMap)) {
                throw new MapUnsuitableException();
            }

            _powerUpGenerator.GeneratePowerUps(gameMap);
        }

        private double DetermineRaduis(int centreX, int centreY, int x, int y) {
            var pointX = centreX - x;
            var pointY = centreY - y;

            if (pointX < 0) {
                pointX = x - centreX;
            }

            if (pointY < 0) {
                pointY = y - centreY;
            }

            return Math.Sqrt(pointX * pointX + pointY * pointY);
        }
    }
}
