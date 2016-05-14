using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities;
using GameEngine.Commands.PlayerCommands;
using GameEngine.Common;
using GameEngine.Engine;
using GameEngine.Exceptions;
using GameEnginetest.DummyObjects;
using GameEnginetest.Entities;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace GameEnginetest.Tests
{
    [TestFixture]
    public class GameRoundProcessorTest
    {
        private GameMap _gameMap;
        private List<Player> _players;
        private DummyLogger _logger = new DummyLogger();

        /// <summary>
        /// Sets up a new game map with two players for testing purposes.
        /// The map only has two players and is configured for a player to be in the top left and bottom right corner
        /// a****
        /// *****
        /// *****
        /// *****
        /// ****b
        /// </summary>
        [SetUp]
        public void InitializeTest()
        {
            _gameMap = new GameMap(5, 5, 0);
            _players = new List<Player> {new TestPlayer("Player 1"), new TestPlayer("Player 2")};

            for (var i = 0; i < _players.Count; i++)
            {
                var player = _players[i];
                var entity = new PlayerEntity()
                {
                    Name = player.Name,
                    Key = (char)('A' + i % 26),
                    BombBag = 1,
                    BombRadius = 1,
                    Killed = false
                };

                player.PlayerRegistered(entity);
            }

            _gameMap.RegisterPlayer(_players[0].PlayerEntity, 1, 1);
            _gameMap.RegisterPlayer(_players[1].PlayerEntity, 5, 5);
        }

        [TearDown]
        public void ResetGameMap()
        {
            InitializeTest();
        }

        [Test]
        public void TestAddPlayerCommand()
        {
            var processor = new GameRoundProcessor(1, _gameMap, _logger);

            var player1 = _players[0];
            var player2 = _players[1];

            processor.AddPlayerCommand(player1, new DoNothingCommand());
            processor.AddPlayerCommand(player1, new PlaceBombCommand());
            Assert.AreEqual(1, processor.CountCommandsRecieved(), "Game processor should not allow multiple commands for the same player");

            player2.PlayerEntity.Killed = true;
            processor.AddPlayerCommand(player2, new PlaceBombCommand());
            Assert.AreEqual(1, processor.CountCommandsRecieved(), "Game processor should not allow killed players to add commands");
            player2.PlayerEntity.Killed = false;

            processor.AddPlayerCommand(player2, new DoNothingCommand());
            Assert.AreEqual(2, processor.CountCommandsRecieved(), "Game processor did not add all of the commands expected");
        }

        [Test]
        public void TestPlayerMoveIntoBombBlast()
        {
            var processor = new GameRoundProcessor(1, _gameMap, _logger);

            var player1 = _players[0];
            var player2 = _players[1];

            MovePlayerToLocation(player1, 4, 4);

            processor.AddPlayerCommand(player2, new PlaceBombCommand());
            processor.ProcessRound();

            var bomb = _gameMap.GetPlayerBombs(player2.PlayerEntity).First();
            while (bomb.BombTimer > 1)
            {
                processor = new GameRoundProcessor(1, _gameMap, _logger);
                processor.ProcessRound();
            }

            processor = new GameRoundProcessor(1, _gameMap, _logger);
            processor.AddPlayerCommand(player1, new MovementCommand(MovementCommand.Direction.Down));
            processor.ProcessRound();

            Assert.IsTrue(player1.PlayerEntity.Killed, "Player 1 should have been killed by bomb blast");
            Assert.IsTrue(player2.PlayerEntity.Killed, "Player 2 should have been killed by bomb blast");
        }

        [Test]
        public void TestPlayersBumbIntoEachOther()
        {
            var processor = new GameRoundProcessor(1, _gameMap, _logger);

            var player1 = _players[0];
            var player2 = _players[1];

            MovePlayerToLocation(player1, 3, 3);
            MovePlayerToLocation(player2, 5, 3);

            processor.AddPlayerCommand(player1, new MovementCommand(MovementCommand.Direction.Right));
            processor.AddPlayerCommand(player2, new MovementCommand(MovementCommand.Direction.Left));

            Assert.True(player1.PlayerEntity.Location.X == 3, "Player 1 should not have moved");
            Assert.True(player2.PlayerEntity.Location.X == 5, "Player 2 should not have moved");
        }

        [Test]
        public void TestPlayersAdjacentPlayersMovingInSameDirection()
        {
            var processor = new GameRoundProcessor(1, _gameMap, _logger);

            var player1 = _players[0];
            var player2 = _players[1];

            MovePlayerToLocation(player1, 1, 1);
            MovePlayerToLocation(player2, 2, 1);

            processor.AddPlayerCommand(player1, new MovementCommand(MovementCommand.Direction.Right));
            processor.AddPlayerCommand(player2, new MovementCommand(MovementCommand.Direction.Right));

            processor.ProcessRound();

            Assert.True(player1.PlayerEntity.Location.X == 2, "Player 1 should have moved");
            Assert.True(player2.PlayerEntity.Location.X == 3, "Player 2 should have moved");
        }

        [Test]
        public void TestPlayersAdjacentPlayersMovingInSameDirectionFail()
        {
            var processor = new GameRoundProcessor(1, _gameMap, _logger);

            var player1 = _players[0];
            var player2 = _players[1];

            MovePlayerToLocation(player1, 1, 1);
            MovePlayerToLocation(player2, 2, 1);

            processor.AddPlayerCommand(player1, new MovementCommand(MovementCommand.Direction.Right));
            processor.AddPlayerCommand(player2, new DoNothingCommand());

            processor.ProcessRound();

            Assert.True(player1.PlayerEntity.Location.X == 1, "Player 1 should not have moved");
            Assert.True(player2.PlayerEntity.Location.X == 2, "Player 2 should not have moved");
        }

        [Test]
        public void TestPlayersAdjacentPlayersMovingTowardsEachOther()
        {
            var processor = new GameRoundProcessor(1, _gameMap, _logger);

            var player1 = _players[0];
            var player2 = _players[1];

            MovePlayerToLocation(player1, 1, 1);
            MovePlayerToLocation(player2, 2, 1);

            processor.AddPlayerCommand(player1, new MovementCommand(MovementCommand.Direction.Right));
            processor.AddPlayerCommand(player2, new MovementCommand(MovementCommand.Direction.Left));

            processor.ProcessRound();

            Assert.True(player1.PlayerEntity.Location.X == 1, "Player 1 should not have moved");
            Assert.True(player2.PlayerEntity.Location.X == 2, "Player 2 should not have moved");
        }

        [Test]
        public void TestPlayerMovingIntoWall()
        {
            var processor = new GameRoundProcessor(1, _gameMap, _logger);

            var player1 = _players[0];
            var player2 = _players[1];

            MovePlayerToLocation(player1, 1, 1);
            MovePlayerToLocation(player2, 2, 1);

            _gameMap.GetBlockAtLocation(3, 1).SetEntity(new IndestructibleWallEntity());

            processor.AddPlayerCommand(player1, new MovementCommand(MovementCommand.Direction.Right));
            processor.AddPlayerCommand(player2, new MovementCommand(MovementCommand.Direction.Right));

            processor.ProcessRound();

            Assert.True(player1.PlayerEntity.Location.X == 1, "Player 1 should not have moved");
            Assert.True(player2.PlayerEntity.Location.X == 2, "Player 2 should not have moved");
        }

        [Test]
        public void TestBombChaining()
        {
            var processor = new GameRoundProcessor(1, _gameMap, _logger);

            var player1 = _players[0];
            var player2 = _players[1];

            player1.PlayerEntity.BombRadius = 4;

            MovePlayerToLocation(player1, 3, 3);
            MovePlayerToLocation(player2, 5, 3);

            processor.AddPlayerCommand(player1, new PlaceBombCommand());
            processor.ProcessRound();

            processor = new GameRoundProcessor(1, _gameMap, _logger);
            processor.ProcessRound();

            processor = new GameRoundProcessor(1, _gameMap, _logger);
            processor.AddPlayerCommand(player2, new PlaceBombCommand());
            processor.ProcessRound();

            var bomb = _gameMap.GetPlayerBombs(player1.PlayerEntity).First();
            var bomb2 = _gameMap.GetPlayerBombs(player2.PlayerEntity).First();
            while (bomb.BombTimer > 0)
            {
                processor = new GameRoundProcessor(1, _gameMap, _logger);
                processor.ProcessRound();
            }

            Assert.True(bomb.IsExploding, "Player 1 bomb should be exploding");
            Assert.True(bomb2.IsExploding, "Player 2 bomb should be exploding");
        }

        [Test]
        public void TwoBombsSameEntity()
        {
            var block = _gameMap.GetBlockAtLocation(3, 3);
            block.SetEntity(new DestructibleWallEntity());

            var processor = new GameRoundProcessor(1, _gameMap, _logger);

            var player1 = _players[0];
            var player2 = _players[1];


            block = _gameMap.GetBlockAtLocation(player1.PlayerEntity.Location.X, player1.PlayerEntity.Location.Y);
            block.SetEntity(null);
            block = _gameMap.GetBlockAtLocation(player2.PlayerEntity.Location.X, player2.PlayerEntity.Location.Y);
            block.SetEntity(null);

            block = _gameMap.GetBlockAtLocation(2, 3);
            block.SetEntity(player1.PlayerEntity);
            block = _gameMap.GetBlockAtLocation(4, 3);
            block.SetEntity(player2.PlayerEntity);

            processor.AddPlayerCommand(player1, new PlaceBombCommand());
            processor.AddPlayerCommand(player2, new PlaceBombCommand());
            processor.ProcessRound();

            block = _gameMap.GetBlockAtLocation(player1.PlayerEntity.Location.X, player1.PlayerEntity.Location.Y);
            block.SetEntity(null);
            block = _gameMap.GetBlockAtLocation(player2.PlayerEntity.Location.X, player2.PlayerEntity.Location.Y);
            block.SetEntity(null);

            block = _gameMap.GetBlockAtLocation(3, 1);
            block.SetEntity(player1.PlayerEntity);
            block = _gameMap.GetBlockAtLocation(2, 1);
            block.SetEntity(player2.PlayerEntity);

            var bomb = _gameMap.GetPlayerBombs(player1.PlayerEntity).FirstOrDefault();
            while (bomb != null && bomb.BombTimer > 0)
            {
                processor = new GameRoundProcessor(0, _gameMap, _logger);
                processor.ProcessRound();
            }

            Assert.AreEqual(player1.PlayerEntity.Points - player1.PlayerEntity.MapCoveragePoints, 10, "Player 1 should have recieved points for destroying wall entity");
            Assert.AreEqual(player2.PlayerEntity.Points - player2.PlayerEntity.MapCoveragePoints, 10, "Player 2 should have recieved points for destroying wall entity");
        }

        [Test]
        public void TwoBombsPersonSuicide()
        {
            var processor = new GameRoundProcessor(1, _gameMap, _logger);

            var player1 = _players[0];
            var player2 = _players[1];

            var block = _gameMap.GetBlockAtLocation(player1.PlayerEntity.Location.X, player1.PlayerEntity.Location.Y);
            block.SetEntity(null);
            block = _gameMap.GetBlockAtLocation(player2.PlayerEntity.Location.X, player2.PlayerEntity.Location.Y);
            block.SetEntity(null);

            block = _gameMap.GetBlockAtLocation(2, 3);
            block.SetEntity(player1.PlayerEntity);
            block = _gameMap.GetBlockAtLocation(4, 3);
            block.SetEntity(player2.PlayerEntity);

            processor.AddPlayerCommand(player1, new PlaceBombCommand());
            processor.AddPlayerCommand(player2, new PlaceBombCommand());
            processor.ProcessRound();

            block = _gameMap.GetBlockAtLocation(player1.PlayerEntity.Location.X, player1.PlayerEntity.Location.Y);
            block.SetEntity(null);
            block = _gameMap.GetBlockAtLocation(player2.PlayerEntity.Location.X, player2.PlayerEntity.Location.Y);
            block.SetEntity(null);

            block = _gameMap.GetBlockAtLocation(3, 1);
            block.SetEntity(player1.PlayerEntity);
            block = _gameMap.GetBlockAtLocation(3, 3);
            block.SetEntity(player2.PlayerEntity);

            var bomb = _gameMap.GetPlayerBombs(player1.PlayerEntity).FirstOrDefault();
            while (bomb != null && bomb.BombTimer > 0)
            {
                processor = new GameRoundProcessor(0, _gameMap, _logger);
                processor.ProcessRound();
            }

            Assert.AreEqual(player1.PlayerEntity.Points, 112, "Player 1 should have recieved points for kiling player 2");
            Assert.AreEqual(player2.PlayerEntity.Points, -88, "Player 2 should have lost points for killing itself");
        }

        [Test]
        public void ChainBombPointAllocations()
        {
            var block = _gameMap.GetBlockAtLocation(1, 3);
            block.SetEntity(new DestructibleWallEntity());

            var processor = new GameRoundProcessor(1, _gameMap, _logger);

            var player1 = _players[0];
            var player2 = _players[1];

            player1.PlayerEntity.BombRadius = 5;
            player2.PlayerEntity.BombRadius = 5;

            MovePlayerToLocation(player1, 2, 3);
            MovePlayerToLocation(player2, 5, 3);

            processor.AddPlayerCommand(player2, new PlaceBombCommand(10));
            processor.ProcessRound();

            var bomb = _gameMap.GetPlayerBombs(player2.PlayerEntity).FirstOrDefault();

            processor = new GameRoundProcessor(1, _gameMap, _logger);
            processor.AddPlayerCommand(player1, new PlaceBombCommand(10));
            processor.ProcessRound();

            MovePlayerToLocation(player1, 3, 2);
            MovePlayerToLocation(player2, 4, 2);

            while (bomb != null && !bomb.IsExploding)
            {
                processor = new GameRoundProcessor(1, _gameMap, _logger);
                processor.ProcessRound();
            }
            //Process another round to assign points
            processor = new GameRoundProcessor(1, _gameMap, _logger);
            processor.ProcessRound();

            Assert.AreEqual(10, player1.PlayerEntity.Points - player1.PlayerEntity.MapCoveragePoints, "Player 1 should have recieved points for destroying wall");
            Assert.AreEqual(10, player2.PlayerEntity.Points - player2.PlayerEntity.MapCoveragePoints, "Player 2 should have recieved points for destroying wall");
        }

        [Test]
        public void PlayerMovementBonus()
        {
            var processor = new GameRoundProcessor(1, _gameMap, _logger);

            var player1 = _players[0];
            var player2 = _players[1];

            _gameMap.GetBlockAtLocation(5, 1).SetEntity(new IndestructibleWallEntity());

            var block = _gameMap.GetBlockAtLocation(player2.PlayerEntity.Location.X, player2.PlayerEntity.Location.Y);
            block.SetEntity(null);
            for (var x = 1; x <= 5; x++)
            {
                for (var y = 1; y <= 5; y++)
                {
                    if(x ==5 && y == 1)
                        continue;
                    MovePlayerToLocation(player1, x, y);
                }
            } 
            block = _gameMap.GetBlockAtLocation(player1.PlayerEntity.Location.X, player1.PlayerEntity.Location.Y);
            block.SetEntity(null);
            block.SetEntity(player2.PlayerEntity);
            for (var x = 1; x <= 5; x++)
            {
                for (var y = 1; y <= 5; y++)
                {
                    if (x == 5 && y == 1)
                        continue;
                    MovePlayerToLocation(player2, x, y);
                }
            }
            processor.ProcessRound();

            Assert.AreEqual(player1.PlayerEntity.Points, 100, "Player 1 should have recieved movement bonus");
            Assert.AreEqual(player2.PlayerEntity.Points, 100, "Player 2 should have recieved movement bonus");
        }

        private void MovePlayerToLocation(Player player, int x, int y)
        {
            while (player.PlayerEntity.Location.X != x || player.PlayerEntity.Location.Y != y)
            {
                var processor = new GameRoundProcessor(0, _gameMap, _logger);

                if (player.PlayerEntity.Location.X > x)
                {
                    processor.AddPlayerCommand(player, new MovementCommand(MovementCommand.Direction.Left));
                    processor.ProcessRound();
                    continue;
                }
                if (player.PlayerEntity.Location.X < x)
                {
                    processor.AddPlayerCommand(player, new MovementCommand(MovementCommand.Direction.Right));
                    processor.ProcessRound();
                    continue;
                }
                if (player.PlayerEntity.Location.Y > y)
                {
                    processor.AddPlayerCommand(player, new MovementCommand(MovementCommand.Direction.Up));
                    processor.ProcessRound();
                    continue;
                }
                if (player.PlayerEntity.Location.Y < y)
                {
                    processor.AddPlayerCommand(player, new MovementCommand(MovementCommand.Direction.Down));
                    processor.ProcessRound();
                    continue;
                }
            }
        }
    }
}
