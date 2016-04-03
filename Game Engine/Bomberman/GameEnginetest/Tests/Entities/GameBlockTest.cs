using System;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.PowerUps;
using NUnit.Framework;

namespace GameEnginetest.Tests.Entities
{
    [TestFixture]
    public class GameBlockTest
    {
        public static GameBlock CreateGameBlock()
        {
            var block = new GameBlock(1, 1);
            return block;
        }

        [Test]
        public void TestBlockLocation()
        {
            var block = CreateGameBlock();
            Assert.AreEqual(block.Location.X, 1, "Game block location incorrect");
            Assert.AreEqual(block.Location.Y, 1, "Game block location incorrect");
        }

        [Test]
        public void TestBlockSetEntity()
        {
            var block = CreateGameBlock();

            var entity = new PlayerEntity();

            block.SetEntity(entity);

            Assert.AreEqual(block.Entity, entity, "Entity was not set correctly on game block");
            Assert.AreEqual(block.Location, entity.Location, "Entity was not set correctly on game block");
            Assert.IsTrue(block.HasBeenTouchedByPlayer(entity), "Block has not been marked as touched by player entity");

            var newEntity = new DestructibleWallEntity();
            Assert.Throws<InvalidOperationException>(() => block.SetEntity(newEntity),
                "Block allowed entity to be ovveridden");

            block.SetEntity(null);
            Assert.IsNull(block.Entity, "Entity was not cleared from game block");
        }

        [Test]
        public void TestBlockSetPowerUp()
        {
            var block = CreateGameBlock();

            var entity = new BombBagPowerUpEntity();

            Assert.IsNull(block.PowerUpEntity, "Game block did not start with null power up entity");

            block.SetPowerUpEntity(entity);
            Assert.AreEqual(block.PowerUpEntity, entity, "Power up entity was not set on game block");

            entity = new BombBagPowerUpEntity();
            Assert.Throws<InvalidOperationException>(() => block.SetPowerUpEntity(entity),
                "Game block allowed power up entity to be overrriden");
        }

        [Test]
        public void TestBlockApplyPowerUp()
        {
            var block = CreateGameBlock();

            Assert.Throws<InvalidOperationException>(block.ApplyPowerUp, "Should indicate that no player is present");

            var wall = new DestructibleWallEntity();
            block.SetEntity(wall);
            Assert.Throws<InvalidOperationException>(block.ApplyPowerUp, "Should indicate that entity is not a player");

            block.SetEntity(null);
            var player = new PlayerEntity();
            block.SetEntity(player);
            Assert.Throws<InvalidOperationException>(block.ApplyPowerUp, "Should indicate that no power up entity is present");

            var entity = new BombBagPowerUpEntity();
            block.SetPowerUpEntity(entity);
            Assert.AreEqual(block.PowerUpEntity, entity, "Power up entity was not set on game block");
            Assert.AreEqual(block.Location, entity.Location, "Power up location not set correctly");

            block.ApplyPowerUp();

            Assert.IsNull(block.PowerUpEntity, "Power up entity should be removed after having been applied");
        }


        [Test]
        public void TestPlantingBomb()
        {
            var block = CreateGameBlock();

            Assert.Throws<InvalidOperationException>(() => block.PlantBomb(2), "Player Entity should be present when planting a bomb");
            block.SetEntity(new DestructibleWallEntity());
            Assert.Throws<InvalidOperationException>(() => block.PlantBomb(2), "Player Entity should be present when planting a bomb");

            block.SetEntity(null);

            var player = new PlayerEntity();
            block.SetEntity(player);
            Assert.Throws<InvalidOperationException>(() => block.PlantBomb(1), "Bomb timer should always be larger than 1");

            block.PlantBomb(2);
            Assert.Throws<InvalidOperationException>(() => block.PlantBomb(2), "Once a block contains a bomb it should not be able to be replaced");

            Assert.AreEqual(block.Bomb.Owner, player, "Block owner not registered correctly");
            Assert.AreEqual(block.Location, block.Bomb.Location, "Bomb location not set correctly");
            Assert.AreEqual(block.Bomb.BombTimer, 2, "Bomb should be registered with the correct timer");
            Assert.AreEqual(block.Bomb.BombRadius, player.BombRadius, "Bomb raduis should equal player radius");
            Assert.IsFalse(block.Bomb.IsExploding, "Bomb should not be registered with exploding state");
        }


        [Test]
        public void TestRemovingBomb()
        {
            var block = CreateGameBlock();
            var player = new PlayerEntity();
            block.SetEntity(player);
            block.PlantBomb(2);

            Assert.Throws<InvalidOperationException>(block.RemoveBomb,
                "A bomb should not be removed until it has exploded");

            block.Bomb.IsExploding = true;

            block.RemoveBomb();
            Assert.Null(block.Bomb, "Bomb should have been removed from block");
        }

        [Test]
        public void TestPowerUpMapSymbols()
        {
            var block = CreateGameBlock();

            var powerUp = new BombBagPowerUpEntity();
            var wall = new DestructibleWallEntity();
            
            block.SetPowerUpEntity(powerUp);
            block.SetEntity(wall);

            block.ExplodingBombs.Add(new BombEntity());

            Assert.AreEqual('*', block.GetMapSymbol(), "Exploding character should always be first to display");
            block.ExplodingBombs.Clear();
            
            Assert.AreEqual(wall.GetMapSymbol(), block.GetMapSymbol(), "Entities should always be displayed before power ups");
            block.SetEntity(null);

            Assert.AreEqual(powerUp.GetMapSymbol(), block.GetMapSymbol(), "Power up should be shown when no other symbols are present");
            block.SetPowerUpEntity(null);

            Assert.AreEqual(' ', block.GetMapSymbol(), "Empty character should display when no other entities are present on the map");

            var player = new PlayerEntity();
            block.SetEntity(player);
            block.PlantBomb(2);

            block.SetPowerUpEntity(powerUp);

            var currentSymbol = block.Bomb.GetMapSymbol();
            Assert.AreEqual(currentSymbol, block.GetMapSymbol(), "The map symbol for a bomb should be shown");

            var block2 = new GameBlock(4, 4);
            block.SetEntity(null);
            block2.SetEntity(player);

            Assert.AreNotEqual(currentSymbol, block.GetMapSymbol(), "Block symbol for bomb should change if user is moved away");
        }
    }
}
