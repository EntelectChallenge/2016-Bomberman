using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Domain.Common;
using Domain.Entities;
using GameEngine.Commands;
using GameEngine.Common;
using GameEngine.Exceptions;
using GameEngine.Loggers;
using GameEngine.MapGenerator;
using GameEngine.MapGenerator.PowerUpGenerators;
using Domain.Exceptions;
using GameEngine.Properties;

namespace GameEngine.Engine
{
    public class BombermanEngine : ILogger
    {
        public delegate void GameStartedHandler(GameMap gameMap);
        public delegate void RoundCompleteHandler(GameMap gameMap, int round);
        public delegate void GameCompleteHandler(GameMap gameMap, List<Player> leaderBoard);

        public event GameStartedHandler GameStarted;
        public event RoundCompleteHandler RoundComplete;
        public event GameCompleteHandler GameComplete;

        public ILogger _logger = new NullLogger();
        private int _currentRound;
        private GameMap _gameMap;
        private List<Player> _players;
        private GameRoundProcessor _roundProcessor;
        private int _playerKillPoints = Settings.Default.PointsPlayer;

        /// <summary>
        /// Prepares a new game.  This will generate a new game map with the provided seed, and register all of the players on the game map
        /// </summary>
        /// <param name="players">The players to register</param>
        /// <param name="seed">The seed for this map</param>
        /// <exception cref="MapUnsuitableException">Will throw an exception if the seed produces an unsuitable map</exception>
        public void PrepareGame(List<Player> players, int seed)
        {
            _gameMap = (new GameMapGenerator(players)).GenerateGameMap(seed);

            _players = players;
            _currentRound = 0;

            foreach (var player in _players)
            {
                player.CommandListener += player_CommandListener;
            }

            Logger.LogInfo("\tOK!");

            using (var enumerator = _gameMap.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if(enumerator.Current == null)
                        continue;

                    var block = enumerator.Current.Entity as DestructibleWallEntity;
                    if (block != null)
                        _playerKillPoints += Settings.Default.PointsWall;
                }
            }

            //Players should only get slightly more points vs points obtained from destroying walls
            _playerKillPoints /= players.Count;
        }

        /// <summary>
        /// Starts the game.  Should be called after prepare game.  
        /// This will notify all registered players that the game has started, and they can start sending commands to the engine.
        /// </summary>
        public void StartNewGame()
        {
            if(_gameMap == null)
                throw new Exception("Game has not yet been prepared");

            StartNewRound();

            PublishGameStarted();

            foreach (var player in _players)
            {
                player.StartGame(_gameMap);
            }
        }

        /// <summary>
        /// Returns the current game map for this round
        /// </summary>
        /// <returns></returns>
        public GameMap GetGameState()
        {
            return _gameMap;
        }

        public Player Winner
        {
            get { return  LeaderBoard.FirstOrDefault(); }
        }

        public List<Player> LeaderBoard
        {
            get { return _players.OrderBy(x => x.PlayerEntity.Killed).ThenByDescending(x => x.PlayerEntity.Points).ThenBy(x => x.PlayerEntity.KilledRound).ToList(); }
        }

        public ReadOnlyCollection<Player> Players
        {
            get { return new ReadOnlyCollection<Player>(_players); }
        }

        /// <summary>
        /// Player commands will passed on to the round processor.  Each player can only send one command per turn.
        /// Once the command for all of the players have been recieved, the round will be processed.
        /// </summary>
        /// <param name="player">The issuing player</param>
        /// <param name="command">The command to performa</param>
        void player_CommandListener(Player player, ICommand command)
        {
            _roundProcessor.AddPlayerCommand(player, command);

            if (_players.Count(x => !x.PlayerEntity.Killed) == 0)
            {
                //No more players remaing in game
                return;
            }

            if(_roundProcessor.CountCommandsRecieved() == _players.Count(x => !x.PlayerEntity.Killed)) //All players sent their commands, process the round
                ProcessRound();
        }

        /// <summary>
        /// Notify all listeners that the current game round has been completed
        /// </summary>
        private void PublishRoundComplete()
        {
            if (RoundComplete != null)
                RoundComplete(_gameMap, _currentRound);
        }

        /// <summary>
        /// Notify all listeners that the current game round has started
        /// </summary>
        private void PublishGameStarted()
        {
            if (GameStarted != null)
                GameStarted(_gameMap);
        }

        private void PublishGameComplete()
        {
            LogInfo("Game has ended, the winnig player is" + (Winner == null ? "no one, game ended in a draw" : Winner.Name));
            LogInfo("Leader Board");
            for (int i = 0; i < LeaderBoard.Count; i++)
            {
                LogInfo(i + ": " + LeaderBoard[i]);
            }

            foreach (var player in _players)
            {
                player.GameEnded(_gameMap);
            }

            if (GameComplete != null)
                GameComplete(_gameMap, LeaderBoard);
        }

        /// <summary>
        /// Processes this current round, then notifies all players of round completion.
        /// </summary>
        private void ProcessRound()
        {
            _roundProcessor.ProcessRound();

            foreach (var player in _players)
            {
                player.RoundComplete(_gameMap, _currentRound);
            }

            PublishRoundComplete();
            StartNewRound();

            if (_gameMap.RegisteredPlayerEntities.Count(x => !x.Killed) < 2)
            {
                PublishGameComplete();
                return;
            }

            if (_currentRound > (_gameMap.MapHeight*_gameMap.MapWidth))
            {
                PublishGameComplete();
                return;
            }

            foreach (var player in _players)
            {
                player.NewRoundStarted(_gameMap);
            }
        }

        private void StartNewRound()
        {
            foreach (var player in _players.Where(x => _roundProcessor != null && _roundProcessor.KillPlayerEntities.Contains(x.PlayerEntity)))
            {
                player.PlayerKilled(_gameMap);
            }

            _currentRound++;
            _roundProcessor = new GameRoundProcessor(_currentRound, _gameMap, Logger, _playerKillPoints);
            _gameMap.CurrentRound = _currentRound;
        }

        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public void LogDebug(string message)
        {
            Logger.LogDebug(message);

            foreach (var player in _players.Where(player => player.Logger != null))
            {
                player.Logger.LogDebug(message);
            }
        }

        public void LogInfo(string message)
        {
            Logger.LogInfo(message);

            foreach (var player in _players.Where(player => player.Logger != null))
            {
                player.Logger.LogInfo(message);
            }
        }

        public void LogException(string message)
        {
            Logger.LogException(message);

            foreach (var player in _players.Where(player => player.Logger != null))
            {
                player.Logger.LogException(message);
            }
        }

        public void LogException(Exception ex)
        {
            Logger.LogException(ex);

            foreach (var player in _players.Where(player => player.Logger != null))
            {
                player.Logger.LogException(ex);
            }
        }

        public void LogException(string message, Exception ex)
        {
            Logger.LogException(message, ex);

            foreach (var player in _players.Where(player => player.Logger != null))
            {
                player.Logger.LogException(message, ex);
            }
        }

        public string ReadAll()
        {
            return Logger.ReadAll();
        }
    }
}
