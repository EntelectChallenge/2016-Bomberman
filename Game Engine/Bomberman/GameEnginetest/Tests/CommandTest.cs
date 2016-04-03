using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities;
using GameEngine.Commands;
using GameEngine.Commands.PlayerCommands;
using GameEngine.Exceptions;
using GameEnginetest.DummyObjects;
using GameEnginetest.Tests.Entities;
using NUnit.Framework;

namespace GameEnginetest.Tests
{
    [TestFixture]
    public class CommandTest
    {
        [Test]
        public void TestPlayerMovementCommand()
        {
            var gameMap = GameMapTest.GenerateTestMap();
            var player = gameMap.RegisteredPlayerEntities.First();
            var transaction = new CommandTransaction(new DummyLogger());

            var playerLocation = player.Location;

            var block = gameMap.GetBlockAtLocation(player.Location.X, player.Location.Y);
            block.PlantBomb(2);

            var command = new MovementCommand(MovementCommand.Direction.Up);

            Assert.Throws<InvalidCommandException>(() => command.PerformCommand(gameMap, player, transaction),
                "Player should not be able to occupy the space of another entity");
            command = new MovementCommand(MovementCommand.Direction.Left);
            Assert.Throws<InvalidCommandException>(() => command.PerformCommand(gameMap, player, transaction),
                "Player should not be able to occupy the space of another entity");

            command = new MovementCommand(MovementCommand.Direction.Right);
            command.PerformCommand(gameMap, player, transaction);

            Assert.Throws<InvalidCommandException>(() => command.PerformCommand(gameMap, player, transaction),
                "Player can only performa one command per transaction");

            transaction.ValidateCommands(gameMap);
            transaction.ProcessCommands(gameMap);

            Assert.AreEqual(playerLocation.Y, player.Location.Y, "Player moved in the wrong direction");
            Assert.AreEqual(playerLocation.X + 1, player.Location.X, "Player moved in the wrond direction");

            command = new MovementCommand(MovementCommand.Direction.Left);
            Assert.Throws<InvalidCommandException>(() => command.PerformCommand(gameMap, player, transaction),
                "Player should not be able to occupy the space if a bomb is in the location");


            gameMap = GameMapTest.GenerateTestMap();
            player = gameMap.RegisteredPlayerEntities.First();
            transaction = new CommandTransaction(new DummyLogger());

            transaction = new CommandTransaction(new DummyLogger());
            command = new MovementCommand(MovementCommand.Direction.Down);
            command.PerformCommand(gameMap, player, transaction);
            transaction.ValidateCommands(gameMap);
            transaction.ProcessCommands(gameMap);
            Assert.AreEqual(playerLocation.Y+ 1, player.Location.Y, "Player moved in the wrong direction");
            Assert.AreEqual(playerLocation.X, player.Location.X, "Player moved in the wrond direction");
        }

        [Test]
        public void TestPlayerCollidingMovementCommand()
        {
            var map = new GameMap(3, 1, 0);

            var player1 = new PlayerEntity();
            var player2 = new PlayerEntity();

            var block = map.GetBlockAtLocation(1, 1);
            block.SetEntity(player1);

            block = map.GetBlockAtLocation(3, 1);
            block.SetEntity(player2);

            var transaction = new CommandTransaction(new DummyLogger());
            new MovementCommand(MovementCommand.Direction.Right).PerformCommand(map, player1, transaction);
            new MovementCommand(MovementCommand.Direction.Left).PerformCommand(map, player2, transaction);

            transaction.ValidateCommands(map);
            transaction.ProcessCommands(map);

            Assert.AreNotEqual(player1.Location.X, player2.Location.X, "Only one of the players should have moved to the location");
        }

        [Test]
        public void TestPlacingBombCommand()
        {
            var map = new GameMap(2, 1, 0);
            var player1 = new PlayerEntity();
            player1.BombBag = 1;

            var block = map.GetBlockAtLocation(1, 1);
            block.SetEntity(player1);

            var transaction = new CommandTransaction(new DummyLogger());
            var command = new PlaceBombCommand();
            command.PerformCommand(map, player1, transaction);
            transaction.ValidateCommands(map);
            transaction.ProcessCommands(map);

            transaction = new CommandTransaction(new DummyLogger());
            command = new PlaceBombCommand();
            Assert.Throws<InvalidCommandException>(() => command.PerformCommand(map, player1, transaction),
                "Cannot place bomb in same location twice");

            var moveCommand = new MovementCommand(MovementCommand.Direction.Right);
            moveCommand.PerformCommand(map, player1, transaction);
            transaction.ValidateCommands(map);
            transaction.ProcessCommands(map);

            transaction = new CommandTransaction(new DummyLogger());
            command = new PlaceBombCommand();
            Assert.Throws<InvalidCommandException>(() => command.PerformCommand(map, player1, transaction),
                "Player cannot place more bombs than is in the bomb bag");
        }
    }
}
