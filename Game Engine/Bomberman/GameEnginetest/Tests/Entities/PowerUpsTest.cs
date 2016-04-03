using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.PowerUps;
using NUnit.Framework;

namespace GameEnginetest.Tests.Entities
{
    [TestFixture]
    public class PowerUpsTest
    {
        [Test]
        public void TestBombagPowerUp()
        {
            var block = new GameBlock(1, 1);
            var player = new PlayerEntity();
            var powerup = new BombBagPowerUpEntity();

            player.BombBag = 1;
            player.BombRadius = 1;

            block.SetEntity(player);
            block.SetPowerUpEntity(powerup);
            block.ApplyPowerUp();

            Assert.AreEqual(2, player.BombBag, "Expected player bombag to increase after power up was applied");
        }

        [Test]
        public void TestBombagPowerRadius()
        {
            var block = new GameBlock(1, 1);
            var player = new PlayerEntity();
            var powerup = new BombRaduisPowerUpEntity();

            player.BombBag = 1;
            player.BombRadius = 2;

            block.SetEntity(player);
            block.SetPowerUpEntity(powerup);
            block.ApplyPowerUp();

            Assert.AreEqual(4, player.BombRadius, "Expected player bom radius to be increased after power up");
        }
    }
}
