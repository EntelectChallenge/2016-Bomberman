using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Common;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using GameEngine.Commands;
using GameEngine.Common;
using GameEngine.Engine.Graphs;
using GameEngine.Exceptions;
using GameEngine.Loggers;
using GameEngine.Properties;
using InvalidOperationException = Domain.Exceptions.InvalidOperationException;

namespace GameEngine.Engine
{
    public class GameRoundProcessor
    {
        private readonly HashSet<IEntity> _entitiesToDestroy = new HashSet<IEntity>();
        private readonly HashSet<PlayerEntity> _killPlayerEntities = new HashSet<PlayerEntity>(); 
        private readonly Graph<BombEntity> _bombGraph = new Graph<BombEntity>(); 
        private readonly Dictionary<Player, ICommand> _commandsToProcess = new Dictionary<Player, ICommand>();
        private readonly GameMap _gameMap;
        private readonly int _round;
        private readonly ILogger _logger;
        private readonly int _playerKillPoints;
        private bool _roundProcessed = false;

        public GameRoundProcessor(int round, GameMap gameMap, ILogger logger)
            :this(round, gameMap, logger, Settings.Default.PointsPlayer)
        {
        }

        public GameRoundProcessor(int round, GameMap gameMap, ILogger logger, int playerKillPoints)
        {
            this._round = round;
            _gameMap = gameMap;
            _logger = logger;
            _playerKillPoints = playerKillPoints;
        }

        public GameMap GameMap
        {
            get { return _gameMap; }
        }

        public int Round
        {
            get { return _round; }
        }

        public HashSet<PlayerEntity> KillPlayerEntities
        {
            get { return _killPlayerEntities; }
        }

        /// <summary>
        /// Adds the command for the player.  Players can only submit one command.  Players can only submit commands if they have not been killed.
        /// </summary>
        /// <param name="player">The issuing player</param>
        /// <param name="command">The command to process</param>
        public void AddPlayerCommand(Player player, ICommand command)
        {
            try
            {
                if (_commandsToProcess.ContainsKey(player))
                    throw new InvalidCommandException(
                        "Player already has a command registered for this round, wait for the next round before sending a new command");

                if (player.PlayerEntity.Killed)
                    throw new InvalidCommandException("Player has been killed and can longer send commands");

                _logger.LogInfo(String.Format("Added Command {0} for Player {1}", command, player.PlayerEntity));
                _commandsToProcess.Add(player, command);
            }
            catch (InvalidCommandException ice)
            {
                player.PlayerCommandFailed(command, ice.Message);
            }
        }

        /// <summary>
        /// The amount of commands recieved by this processor
        /// </summary>
        /// <returns></returns>
        public int CountCommandsRecieved()
        {
            return _commandsToProcess.Count();
        }

        /// <summary>
        /// Performs all of the processing logic for this round of play
        /// </summary>
        public void ProcessRound()
        {
            if (_roundProcessed)
            {
                throw new InvalidOperationException("This round has already been processed!");
            }
            _logger.LogDebug("Beginning round processing");
            RemoveExplosionsFromMap();
            DescreaseBombTimers();
            DetonateBombs();
            MarkEntitiesForDestruction();
            ProcessPlayerCommands();
            ApplyPowerUps();
            DestroyMarkedEntities();
            ApplyMovementBonus();
            _logger.LogDebug("Round processing complete");
            _roundProcessed = true;
        }

        /// <summary>
        /// Remove all explosion states from the map.
        /// </summary>
        protected void RemoveExplosionsFromMap()
        {
            _logger.LogDebug("Removing Bomb Explosions from map");
            using (var enumerator = _gameMap.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var gameBlock = enumerator.Current;
                    if (gameBlock == null) continue;

                    gameBlock.ExplodingBombs.Clear();
                }
            }
        }

        /// <summary>
        /// Decrease the timers for all of the bombs on the map
        /// </summary>
        protected void DescreaseBombTimers()
        {
            _logger.LogDebug("Decreasing Bomb Timers");
            using (var enumerator = _gameMap.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var gameBlock = enumerator.Current;
                    if (gameBlock == null) continue;

                    if (gameBlock.Bomb != null)
                    {
                        gameBlock.Bomb.BombTimer--;
                    }
                }
            }
        }

        /// <summary>
        /// Detonate all of the bombs with a timer value of zero
        /// </summary>
        protected void DetonateBombs()
        {
            _logger.LogDebug("Detonating Bombs");
            using (var enumerator = _gameMap.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var gameBlock = enumerator.Current;
                    if (gameBlock == null) continue;

                    if (gameBlock.Bomb != null && gameBlock.Bomb.BombTimer < 1)
                    {
                        DetonateBomb(gameBlock.Bomb);
                    }
                }
            }
        }

        /// <summary>
        /// Detonates a specific bomb entity.  
        /// This calculates the blast raduis for the bomb, and marks all of the game blocks affected by the explosion.
        /// </summary>
        /// <param name="bombEntity">The bomb to detonate</param>
        protected void DetonateBomb(BombEntity bombEntity)
        {
            _logger.LogInfo(String.Format("Detonating Bomb {0}", bombEntity));
            _bombGraph.AddNode(bombEntity);

            if (bombEntity.IsExploding)
                return;

            bombEntity.IsExploding = true;

            var bombX = bombEntity.Location.X;
            var bombY = bombEntity.Location.Y;
            var bombRaduis = bombEntity.BombRadius;

            for (var i = bombX; i <= bombX + bombRaduis; i++)
            {
                if (i > _gameMap.MapWidth)
                    break;

                if (!MarkGameBlockExploded(i, bombY, bombEntity))
                    break;
            }

            for (var i = bombX; i >= bombX - bombRaduis; i--)
            {
                if (i < 1)
                    break;

                if (!MarkGameBlockExploded(i, bombY, bombEntity))
                    break;
            }

            for (var i = bombY; i <= bombY + bombRaduis; i++)
            {
                if (i > _gameMap.MapHeight)
                    break;

                if (!MarkGameBlockExploded(bombX, i, bombEntity))
                    break;
            }

            for (var i = bombY; i >= bombY - bombRaduis; i--)
            {
                if (i < 1)
                    break;

                if (!MarkGameBlockExploded(bombX, i, bombEntity))
                    break;
            }
            
        }

        /// <summary>
        /// Marks the game block at the specified location as exploded.
        /// If the block contains a bomb that is not yet exploding, that bomb will then be detonated.
        /// </summary>
        /// <param name="x">X Location</param>
        /// <param name="y">Y Location</param>
        /// <param name="bomb">The bomb causing the explostion</param>
        /// <returns>True if the block was marked succesfully, false if the block contains a non destructable entity</returns>
        protected bool MarkGameBlockExploded(int x, int y, BombEntity bomb)
        {

            var gameBlock = _gameMap.GetBlockAtLocation(x, y);

            if (gameBlock.Entity == null)
            {
                gameBlock.ExplodingBombs.Add(bomb);
            }
            if (gameBlock.Bomb != null && gameBlock.Bomb != bomb)
            {
                DetonateBomb(gameBlock.Bomb);
                _bombGraph.ConnectNodes(bomb, gameBlock.Bomb);
                return false;
            }
            if (gameBlock.Entity != null)
            {
                var entity = gameBlock.Entity;
                if (entity.IsDestructable()) { gameBlock.ExplodingBombs.Add(bomb); }
                if (entity.StopsBombBlast()) {return false;}
            }

            return true;
        }

        /// <summary>
        /// Marks all entities on exploding game blocks for destruction.
        /// Entities will only be marked if they are destructable
        /// </summary>
        protected void MarkEntitiesForDestruction()
        {
            using (var enumerator = _gameMap.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var gameBlock = enumerator.Current;
                    if (gameBlock == null) continue;

                    if (gameBlock.Exploding && gameBlock.Entity != null && gameBlock.Entity.IsDestructable())
                    {
                        _entitiesToDestroy.Add(gameBlock.Entity);

                        if (gameBlock.Entity.GetType() != typeof (PlayerEntity)) continue;
                        var player = (PlayerEntity) gameBlock.Entity;
                        player.Killed = true;
                        player.KilledRound = Round;
                    }
                }
            }
        }

        /// <summary>
        /// Process the player commands.  Player commands are processed in a commands transaction,
        /// the transaction is verified, removing invalid commands, and then the commands are persisted onto the game map
        /// </summary>
        protected void ProcessPlayerCommands()
        {
            _logger.LogDebug("Processing Player Commands");
            var hadCommands = _commandsToProcess.Any();

            var transaction = new CommandTransaction(_logger);
            foreach (var command in _commandsToProcess)
            {
                if (command.Key.PlayerEntity.Killed || _entitiesToDestroy.Contains(command.Key.PlayerEntity))
                {
                    _logger.LogInfo(String.Format("Player {0} has been killed, and the command {1} will be ignored", command.Key.PlayerEntity, command.Value));
                    continue;
                }

                try
                {
                    command.Value.PerformCommand(_gameMap, command.Key.PlayerEntity, transaction);
                }
                catch (InvalidCommandException ex)
                {
                    command.Key.PlayerCommandFailed(command.Value, ex.Message);
                    _logger.LogException(String.Format("Failed to process command {0} for player {1}", command.Value, command.Key.PlayerEntity), ex);
                }
            }
            transaction.ValidateCommands(_gameMap);
            transaction.ProcessCommands(_gameMap);

            _commandsToProcess.Clear();

            //Do another round of marking entities for destruction, just incase a player moved into a bomb blast
            if(hadCommands) MarkEntitiesForDestruction();
        }
        
        /// <summary>
        /// Apply any power ups for players standing on one.
        /// </summary>
        protected void ApplyPowerUps()
        {
            _logger.LogDebug("Applying player power ups");
            using (var enumerator = _gameMap.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var gameBlock = enumerator.Current;
                    if (gameBlock == null) continue;

                    if (gameBlock.Entity != null && gameBlock.PowerUpEntity != null && gameBlock.Entity.GetType() == typeof(PlayerEntity))
                    {
                        gameBlock.ApplyPowerUp();
                    }
                }
            }
        }

        /// <summary>
        /// Destroy all entites marked for destruction, and remove them from the game map.
        /// </summary>
        protected void DestroyMarkedEntities()
        {
            _logger.LogDebug("Destroying marked entities");
            using (var enumerator = _gameMap.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var gameBlock = enumerator.Current;
                    if (gameBlock == null) continue;

                    if (_entitiesToDestroy.Contains(gameBlock.Entity))
                    {
                        if (gameBlock.Entity.GetType() == typeof (PlayerEntity))
                        {
                            var player = (PlayerEntity) gameBlock.Entity;
                            player.Killed = true;
                            player.RemovePoints(_playerKillPoints);
                            KillPlayerEntities.Add(player);
                        }

                        foreach (var explodingBomb in gameBlock.ExplodingBombs)
                        {
                            AssignPoints(explodingBomb, gameBlock.Entity);
                        }

                        _logger.LogDebug(String.Format("Destroyed Entity {0}", gameBlock.Entity));
                        gameBlock.SetEntity(null);
                    }
                    if (gameBlock.Bomb != null && gameBlock.Bomb.IsExploding)
                    {
                        gameBlock.RemoveBomb();
                    }
                }
            }

            CalculateBombBlastPoints();

            _entitiesToDestroy.Clear();
            _bombGraph.NodeSet.Clear();
        }

        protected void AssignPoints(BombEntity ownerBomb, IEntity destroyedEntity)
        {
            if (destroyedEntity == null)
            {
                return;
            }
            if (destroyedEntity.GetType() == typeof(PlayerEntity))
            {
                ownerBomb.Points += _playerKillPoints;
                return;
            } 
            if (destroyedEntity.GetType() == typeof(DestructibleWallEntity))
            {
                ownerBomb.Points += Settings.Default.PointsWall;
                return;
            }
        }

        protected void CalculateBombBlastPoints()
        {
            var visitor = new PointsVisitor();
            _bombGraph.VisitGroups(visitor);
        }

        protected void ApplyMovementBonus()
        {
            //Reset all movement bonuses
            foreach (var player in _gameMap.RegisteredPlayerEntities.Where(x => !x.Killed))
            {
                player.MapCoveragePoints = 0;
            }

            int usableBlocks = 0;
             
            using (var enumerator = _gameMap.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var gameBlock = enumerator.Current;
                    if (gameBlock == null) continue;

                    if (gameBlock.Entity == null || gameBlock.Entity.GetType() != typeof (IndestructibleWallEntity))
                    {
                        usableBlocks++;
                    }

                    foreach (var player in _gameMap.RegisteredPlayerEntities.Where(x => !x.Killed && gameBlock.HasBeenTouchedByPlayer(x)))
                    {
                        player.MapCoveragePoints++;
                    }
                }
            }

            foreach (var player in _gameMap.RegisteredPlayerEntities.Where(x => !x.Killed))
            {
                var mapCoverage = ((double) player.MapCoveragePoints/usableBlocks);
                player.MapCoveragePoints = (int)(mapCoverage * Settings.Default.PointsMovementMultiplierPercentage);

                _logger.LogInfo(String.Format("Player {0} has a movement coverage of {1:N2}% for the map", player, mapCoverage * 100));
            }
        }
    }
}
