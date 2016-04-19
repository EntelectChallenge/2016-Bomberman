using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Domain.Common;
using Domain.Meta;
using GameEngine.Common;
using GameEngine.Engine;
using GameEngine.Loggers;
using GameEngine.Renderers;
using Newtonsoft.Json;
using TestHarness.TestHarnesses.Bot;
using TestHarness.TestHarnesses.ConsoleHarness;
using TestHarness.Util;

namespace Bomberman
{
    public class BombermanGame
    {
        public ILogger Logger = new InMemoryLogger();
        private String _runLocation;
        private BombermanEngine _engine;

        public void StartNewGame(Options options)
        {
            var players = new List<Player>();
            var gameSeed = options.GameSeed ?? new Random().Next();
           _runLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Replays");

            try
            {
                _engine = new BombermanEngine { Logger = Logger };
                _engine.GameComplete += EngineOnGameComplete;
                _engine.RoundComplete += (map, round) => LogEngineInfo(map, round - 1);

                if (options.Pretty)
                {
                    _engine.RoundComplete += engine_RoundComplete;
                    _engine.GameStarted += EngineOnGameStarted;
                }

                _runLocation = !String.IsNullOrEmpty(options.Log) ? options.Log : Path.Combine(_runLocation, gameSeed.ToString());

                for (var i = 0; i < options.ConsolePlayers; i++)
                {
                    players.Add(new ConsoleHarness("Player " + (players.Count + 1)));
                }

                players.AddRange(options.BotFolders.Select(botFolder => LoadBot(botFolder, _runLocation)).Where(player => player != null));

                if (players.Count == 0)
                {
                    for (var i = 0; i < 4; i++)
                    {
                        players.Add(new ConsoleHarness("Player " + (players.Count + 1)));
                    }
                }

                foreach (var player in players)
                {
                    Logger.LogInfo("Registered player " + player.Name);
                }

                _engine.PrepareGame(players, gameSeed);

                LogEngineInfo(_engine.GetGameState(), 0);
                _engine.StartNewGame();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void EngineOnGameStarted(GameMap gameMap)
        {
            ConsoleRender.RenderToConsolePretty(gameMap, 'Z');
        }

        void engine_RoundComplete(GameMap gameMap, int round)
        {
            ConsoleRender.RenderToConsolePretty(gameMap, 'Z');
        }

        private void EngineOnGameComplete(GameMap gameMap, List<Player> leaderBoard)
        {
            Logger.LogInfo("Game has ended");
            Logger.LogInfo("Leader Board");
            for (int i = 0; i < leaderBoard.Count; i++)
            {
                Logger.LogInfo(i + ": " + leaderBoard[i]);
            }

            Console.WriteLine("Game has ended");
            Console.WriteLine("Leader Board");
            for (int i = 0; i < leaderBoard.Count; i++)
            {
                Console.WriteLine(i + ": " + leaderBoard[i]);
            }
        }

        private void LogEngineInfo(GameMap gameMap, int round)
        {
            var engineLog = Path.Combine(_runLocation, round.ToString());
            var mapLocation = Path.Combine(_runLocation, round.ToString());
            var stateLocation = Path.Combine(_runLocation, round.ToString());
            var roundInfoLocation = Path.Combine(_runLocation, round.ToString());

            if (!Directory.Exists(engineLog))
                Directory.CreateDirectory(engineLog);

            engineLog = Path.Combine(engineLog, "engine.log");
            mapLocation = Path.Combine(mapLocation, "map.txt");
            stateLocation = Path.Combine(stateLocation, "state.json");
            roundInfoLocation = Path.Combine(roundInfoLocation, "roundInfo.json");

            if (File.Exists(engineLog))
                File.Delete(engineLog);
            if (File.Exists(mapLocation))
                File.Delete(mapLocation);
            if (File.Exists(stateLocation))
                File.Delete(stateLocation);
            if (File.Exists(roundInfoLocation))
                File.Delete(roundInfoLocation);

            var renderer = new GameMapRender(gameMap, true);
            var json = renderer.RenderJsonGameState();
            var map = renderer.RenderTextGameState();
            var roundInfo = GenerateRoundInfo(gameMap);

            File.WriteAllText(engineLog, Logger.ReadAll(), new UTF8Encoding(false));
            File.WriteAllText(mapLocation, map.ToString(), new UTF8Encoding(false));
            File.WriteAllText(stateLocation, json.ToString(), new UTF8Encoding(false));
            File.WriteAllText(roundInfoLocation, roundInfo, new UTF8Encoding(false));
        }

        private string GenerateRoundInfo(GameMap gameMap)
        {
            var roundInfo = new RoundInfo()
            {
                MapSeed = gameMap.MapSeed,
                Round = gameMap.CurrentRound,
                Winner = GetPlayerInfo(_engine.Winner),
                Players = _engine.Players.Select(GetPlayerInfo),
                LeaderBoard = _engine.LeaderBoard.Select(GetPlayerInfo)
            };

            return JsonConvert.SerializeObject(roundInfo);
        }

        private PlayerInfo GetPlayerInfo(Player player)
        {
            if (player == null)
                return null;

            var botPlayer = player as BotHarness;
            return new PlayerInfo()
            {
                Email = botPlayer == null ? player.Name : botPlayer.BotMeta.Email,
                NickName = botPlayer == null ? player.Name : botPlayer.BotMeta.NickName,
                Author = botPlayer == null ? player.Name : botPlayer.BotMeta.Author,
                BotType = botPlayer == null ? "Unkown" : botPlayer.BotMeta.BotType.ToString(),
                PlayerKey = player.PlayerEntity.Key
            };
        }

        private Player LoadBot(String botLocation, String logLocation)
        {
            try
            {
                var botMeta = BotMetaReader.ReadBotMeta(botLocation);

                Logger.LogInfo("Loaded bot " + botLocation);

                return new BotHarness(botMeta, botLocation, logLocation);
            }
            catch (Exception ex)
            {
                Logger.LogException("Failed to load bot " + botLocation, ex);
                Console.WriteLine("Failed to load bot " + botLocation + Environment.NewLine + ex);
                return null;
            }
        }
    }
}
