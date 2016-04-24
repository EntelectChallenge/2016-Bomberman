using System.Collections.Generic;
using System.Linq;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.PowerUps;
using Domain.Exceptions;
using GameEngine.Common;
using GameEngine.MapGenerator;
using GameEnginetest.Entities;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace GameEnginetest.Tests.Entities
{
    [TestFixture]
    public class GameMapTest
    {
        public static GameMap GenerateTestMap()
        {
            var players = new List<Player>();
            players.Add(new TestPlayer("Player 1"));
            players.Add(new TestPlayer("Player 2"));
            players.Add(new TestPlayer("Player 3"));
            players.Add(new TestPlayer("Player 4"));

            return new GameMapGenerator(players, true).GenerateGameMap(1);
        }

        [Test]
        public void TestGettingGameBlock()
        {
            var gameMap = GenerateTestMap();

            var block = gameMap.GetBlockAtLocation(1, 1);

            Assert.AreEqual(block.Location.X, 1, "Game block retrieved from wrong location");
            Assert.AreEqual(block.Location.Y, 1, "Game block retrieved from wrong location");
            Assert.Throws<LocationOutOfBoundsException>(() => gameMap.GetBlockAtLocation(0, 0), "Retrieval should throw exception for out of bound coordinates");
            Assert.Throws<LocationOutOfBoundsException>(() => gameMap.GetBlockAtLocation(gameMap.MapWidth + 1, 0), "Retrieval should throw exception for out of bound coordinates");
            Assert.Throws<LocationOutOfBoundsException>(() => gameMap.GetBlockAtLocation(0, gameMap.MapHeight + 1), "Retrieval should throw exception for out of bound coordinates");
            Assert.Throws<LocationOutOfBoundsException>(() => gameMap.GetBlockAtLocation(gameMap.MapWidth + 1, gameMap.MapHeight + 1), "Retrieval should throw exception for out of bound coordinates");

        }

        [Test]
        public void TestGeneratedMap()
        {
            var gameMap = GenerateTestMap();

            Assert.IsTrue(gameMap.RegisteredPlayerEntities.Count == 4, "Not all players have been registered onto the map");
            Assert.AreEqual(1, gameMap.MapSeed);

            GameBlock block;


            block = gameMap.GetBlockAtLocation(2, 2);
            Assert.IsInstanceOf<PlayerEntity>(block.Entity, "Player not at expected location");
            Assert.AreEqual(block.Entity.Location.X, 2, "Player not at expected location");
            Assert.AreEqual(block.Entity.Location.Y, 2, "Player not at expected location");

            block = gameMap.GetBlockAtLocation(gameMap.MapWidth - 1, 2);
            Assert.IsInstanceOf<PlayerEntity>(block.Entity, "Player not at expected location");
            Assert.AreEqual(block.Entity.Location.X, gameMap.MapWidth - 1, "Player not at expected location");
            Assert.AreEqual(block.Entity.Location.Y, 2, "Player not at expected location");

            block = gameMap.GetBlockAtLocation(2, gameMap.MapHeight - 1);
            Assert.IsInstanceOf<PlayerEntity>(block.Entity, "Player not at expected location");
            Assert.AreEqual(block.Entity.Location.X, 2, "Player not at expected location");
            Assert.AreEqual(block.Entity.Location.Y, gameMap.MapHeight - 1, "Player not at expected location");

            block = gameMap.GetBlockAtLocation(gameMap.MapWidth - 1, gameMap.MapHeight - 1);
            Assert.IsInstanceOf<PlayerEntity>(block.Entity, "Player not at expected location");
            Assert.AreEqual(block.Entity.Location.X, gameMap.MapWidth - 1, "Player not at expected location");
            Assert.AreEqual(block.Entity.Location.Y, gameMap.MapHeight - 1, "Player not at expected location");

            //Check that the map has walls around the sides
            for (var x = 1; x <= gameMap.MapWidth; x++)
            {
                block = gameMap.GetBlockAtLocation(x, 1);
                Assert.IsInstanceOf<IndestructibleWallEntity>(block.Entity, "Indestructible Wall Entity expected at location");
                block = gameMap.GetBlockAtLocation(x, gameMap.MapHeight);
                Assert.IsInstanceOf<IndestructibleWallEntity>(block.Entity, "Indestructible Wall Entity expected at location");
            }
            for (var y = 1; y <= gameMap.MapHeight; y++)
            {
                block = gameMap.GetBlockAtLocation(1, y);
                Assert.IsInstanceOf<IndestructibleWallEntity>(block.Entity, "Indestructible Wall Entity expected at location");
                block = gameMap.GetBlockAtLocation(gameMap.MapWidth, y);
                Assert.IsInstanceOf<IndestructibleWallEntity>(block.Entity, "Indestructible Wall Entity expected at location");
            }

            //Check that every second block has an indesctructible wall
            for (var x = 3; x < gameMap.MapWidth -1; x += 2)
            {
                for (var y = 3; y < gameMap.MapHeight -1; y += 2)
                {
                    if(x == (gameMap.MapWidth / 2) + 1 && y == (gameMap.MapHeight / 2) + 1)
                        continue;

                    block = gameMap.GetBlockAtLocation(x, y);
                    Assert.IsInstanceOf<IndestructibleWallEntity>(block.Entity, "Indestructible Wall Entity expected at location");
                }
            }

            //Check the random destructible walls.
            int destructibleWallCheckSum = gameMap.Select(b => b.Entity).OfType<DestructibleWallEntity>().Sum(b => b.Location.Y * b.Location.Y * b.Location.X);
            Assert.AreEqual(204886, destructibleWallCheckSum);

            //Check the random power-ups.
            int powerUpsCheckSum = gameMap.Where(b => b.PowerUpEntity != null).Sum(b => (int)b.PowerUpEntity.GetMapSymbol() * b.Location.Y * b.Location.Y * b.Location.X);
            Assert.AreEqual(1130096, powerUpsCheckSum);

            var bombBags = 0;
            var bombRadii = 0;
            using (var enumerator = gameMap.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var powerUpBlock = enumerator.Current;

                    bombBags += powerUpBlock.PowerUpEntity != null &&
                               powerUpBlock.PowerUpEntity.GetType() == typeof(BombBagPowerUpEntity)
                        ? 1
                        : 0;
                    bombRadii += powerUpBlock.PowerUpEntity != null &&
                               powerUpBlock.PowerUpEntity.GetType() == typeof(BombRaduisPowerUpEntity)
                        ? 1
                        : 0;
                }
            }

            Assert.AreEqual(4 * 2, bombBags, "Too many bomb bags placed on map");
            Assert.AreEqual(4 * 4, bombRadii, "Too many bomb radii placed on map");
        }

        [Test]
        public void TestEnumerator()
        {
            var gameMap = GenerateTestMap();
            
            int x = 1;
            int y = 1;

            using (var enumerator = gameMap.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var block = enumerator.Current;

                    var gameMapBlock = gameMap.GetBlockAtLocation(x, y);

                    Assert.AreEqual(block, gameMapBlock);
                    x++;

                    if (x > gameMap.MapWidth)
                    {
                        x = 1;
                        y++;
                    }
                }
            }
        }

        [Test]
        public void TestPlayerBombCount()
        {
            var gameMap = GenerateTestMap();

            var player = gameMap.RegisteredPlayerEntities.First();
            var bombCount = gameMap.GetPlayerBombCount(player);

            Assert.AreEqual(bombCount, 0, "Player should start with bomb count of 0");

            gameMap.GetBlockAtLocation(player.Location.X, player.Location.Y).PlantBomb(2);
            bombCount = gameMap.GetPlayerBombCount(player);

            Assert.AreEqual(bombCount, 1, "Player bomb count should have increased after planting bomb");

        }
    }
}
