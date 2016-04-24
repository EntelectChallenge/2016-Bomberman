using System;
using System.Collections.Generic;
using System.Linq;
using BomberManUnity.Loggers;
using BomberManUnity.Players;
using Domain.Common;
using GameEngine.Commands.PlayerCommands;
using GameEngine.Common;
using GameEngine.MapGenerator;

namespace BomberManUnity.Engine
{
    public class UnityEngine
    {
        private GameMap _gameMap;
        private UnityRoundProcessor _roundProcessor;
        private List<Player> _players;

        public void PrepareNewGame(List<UnityPlayer> players, int seed)
        {
            _players = players.Cast<Player>().ToList();
            var mapGenerator = new GameMapGenerator(_players);
            _gameMap = mapGenerator.GenerateGameMap(seed);
        }

        public void StartNewGame()
        {
            if (_gameMap == null)
                throw new Exception("Game has not yet been prepared");

            _roundProcessor = new UnityRoundProcessor(0, _gameMap, new EmptyLogger());

            foreach (var player in _players)
            {
                player.StartGame(_gameMap);
            }
        }

        public GameMap GetGameState()
        {
            return _gameMap;
        }

        /// <summary>
        /// This should tick every couple of milliseconds to process player commands etc
        /// </summary>
        public GameMap TickMinor()
        {
            _roundProcessor.TickMinor();

            return _gameMap;
        }

        /// <summary>
        /// Major ticks should occur every second, this will run all of the other round processing stuff like bomb triggering etc
        /// </summary>
        public GameMap TickMajor()
        {
            _roundProcessor.TickMajor();

            return _gameMap;
        }

        public void TriggerBomb(UnityPlayer player)
        {
            _roundProcessor.AddPlayerCommand(player, new TriggerBombCommand());
        }

        public void PlantBomb(UnityPlayer player)
        { 
            _roundProcessor.AddPlayerCommand(player, new PlaceBombCommand());
        }

        public void MovePlayerToLocation(UnityPlayer player, int x, int y)
        {
            _roundProcessor.MovePlayerToLocation(player, x, y);
            
        }
    }
}
