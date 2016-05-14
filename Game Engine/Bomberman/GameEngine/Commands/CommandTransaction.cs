using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Common;
using Domain.Entities;
using GameEngine.Exceptions;
using GameEngine.Loggers;

namespace GameEngine.Commands
{
    public class CommandTransaction
    {
        private readonly ILogger _logger;

        private readonly Dictionary<PlayerEntity, GameBlock> _playerMovementDestinations = new Dictionary<PlayerEntity, GameBlock>();

        public CommandTransaction(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Request the movement of a player on the game map
        /// </summary>
        /// <param name="playerEntity">The player to move</param>
        /// <param name="destination">The destination block for the move</param>
        public void RequestMovement(PlayerEntity playerEntity, GameBlock destination)
        {
            if(_playerMovementDestinations.ContainsKey(playerEntity))
                throw new InvalidCommandException("Command Transaction already contains a destination for player " + playerEntity);

            _playerMovementDestinations.Add(playerEntity, destination);
        }

        /// <summary>
        /// Validates that all commands will succeed without any entity collisions, in the event of collision choose a random entity to win the collision
        /// </summary>
        /// <param name="gameMap">The game map the commands will be performed on</param>
        public void ValidateCommands(GameMap gameMap)
        {
            var sameDestinationBlocks = _playerMovementDestinations.GroupBy(x => x.Value).Where(x => x.Count() > 1);
            var commandsToRemove = sameDestinationBlocks.Select(sameDestinationBlock => sameDestinationBlock.Key).SelectMany(gameBlock => _playerMovementDestinations.Where(x => x.Value == gameBlock)).Select(source => source.Key).ToList();

            if(commandsToRemove.Count == 0)
                return;

            //Randomly choose a player to lose the move command so game can continue
            PlayerEntity loser = null;
            var lowest = 101;
            var random = new Random();

            foreach (var playerEntity in commandsToRemove)
            {
                var playerChance = random.Next(100);
                _logger.LogInfo(String.Format("Player collision detected, player {0} rolled {1} to execute the command", playerEntity.Key, playerChance));
                if (playerChance < lowest)
                {
                    loser = playerEntity;
                    lowest = playerChance;
                }
            }

            if (loser == null)
                return;

            _logger.LogInfo("Player collision detected, player " + loser.Key + " has been chosed as the victim to lose his command");
            _playerMovementDestinations.Remove(loser);

            //Continue calling validate until all colisions have been processed
            ValidateCommands(gameMap);
        }

        /// <summary>
        /// Processes all validated commands on the game map
        /// </summary>
        /// <param name="gameMap">The game map the commands will be performed on</param>
        public void ProcessCommands(GameMap gameMap)
        {
            ValidateCommands(gameMap);

            while (_playerMovementDestinations.Any())
            {
                var playerMovementDestination = _playerMovementDestinations.OrderBy(x => x.Value.Entity != null).First();
                _playerMovementDestinations.Remove(playerMovementDestination.Key);

                _logger.LogInfo(String.Format("Trying to move player {0} to destination {1}", playerMovementDestination.Key.Key, playerMovementDestination.Value.Location));

                var player = playerMovementDestination.Key;
                var playerLocation = player.Location;
                var playerBlock = gameMap.GetBlockAtLocation(playerLocation.X, playerLocation.Y);
                var destinationBlock = playerMovementDestination.Value;

                if (destinationBlock.Entity != null)
                {
                    _logger.LogException(String.Format("Could not move player {0} to destination {1} because another entity occupies this space", playerMovementDestination.Key.Key, playerMovementDestination.Value.Location));
                    continue;
                }

                playerBlock.SetEntity(null);
                destinationBlock.SetEntity(player);
            }
        }
    }
}
