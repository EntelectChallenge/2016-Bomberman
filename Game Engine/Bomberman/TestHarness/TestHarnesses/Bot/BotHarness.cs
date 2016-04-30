using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Domain.Common;
using Domain.Meta;
using GameEngine.Commands;
using GameEngine.Commands.PlayerCommands;
using GameEngine.Common;
using GameEngine.Loggers;
using GameEngine.Renderers;
using TestHarness.Exceptions;
using TestHarness.Properties;
using TestHarness.TestHarnesses.Bot.Runners;

namespace TestHarness.TestHarnesses.Bot
{
    public class BotHarness : Player
    {
        public readonly BotMeta BotMeta;
        public readonly string BotDir;
        public readonly string WorkDir;
        private readonly InMemoryLogger _inMemoryLogger;
        private readonly BotRunner _botRunner;
        private int _currentRound = 0;
        private int _totalDoNothingCommands;

        public BotHarness(BotMeta meta, string botDir, string workDir)
            : base(meta.NickName ?? meta.Author ?? meta.Email)
        {
            BotMeta = meta;
            BotDir = botDir;
            WorkDir = workDir;

            _inMemoryLogger = new InMemoryLogger();
            Logger = _inMemoryLogger;

            switch (meta.BotType)
            {
                case BotMeta.BotTypes.CSharp:
                case BotMeta.BotTypes.CPlusPlus:
                case BotMeta.BotTypes.FSharp:
                    _botRunner = new DotNetRunner(this);
                    break;
                case BotMeta.BotTypes.Python2:
                case BotMeta.BotTypes.Python3:
                    _botRunner = new PythonRunner(this);
                    break;
                case BotMeta.BotTypes.Java:
                    _botRunner = new JavaRunner(this);
                    break;
                case BotMeta.BotTypes.JavaScript:
                    _botRunner = new JavaScriptRunner(this);
                    break;
                default:
                    throw new ArgumentException("Invalid bot type " + meta.BotType);
            }
        }

        public override void StartGame(GameMap gameState)
        {
            WriteRoundFiles(gameState);
            _botRunner.CalibrateBot();
            RemoveMoveFile();//Remove the move file created by calibration bots
            NewRoundStarted(gameState);
        }

        public override void NewRoundStarted(GameMap gameState)
        {
            if (!PlayerEntity.Killed)
            {
                RunBotAndGetNextMove();
            }
        }

        public override void RoundComplete(GameMap gameMap, int round)
        {
            base.RoundComplete(gameMap, round);

            _currentRound = round;
            ClearPreviousRoundFiles();
            WriteRoundFiles(gameMap);
        }

        public override void GameEnded(GameMap gameMap)
        {
            Logger.LogInfo("Game has ended");
            WriteRoundFiles(gameMap);
        }

        public override void PlayerKilled(GameMap gameMap)
        {
            Logger.LogInfo("Player has been killed");
        }

        public override void PlayerCommandFailed(ICommand command, string reason)
        {
            Logger.LogInfo(String.Format("Player Command {0} failed because {1}", command, reason));
        }

        public override void Dispose()
        {
        }

        private void RunBotAndGetNextMove()
        {
            if (_totalDoNothingCommands >= 10)
            {
                Logger.LogInfo(
                    "Bot is sending to many do nothing commands, in order to save the game the player will plant a bomb and kill itself soon");
            }
            if (_totalDoNothingCommands >= 20)
            {
                Logger.LogInfo(
                    "Bot sent to many do nothing commands, something is most likely going wrong, please fix your bot.  Your player will now attempt to kill itself to save the game.");
            }
            if (_totalDoNothingCommands == 21)
            {
                PublishCommand(new TriggerBombCommand());
                return;
            }

            ICommand command;
            try
            {
                _botRunner.RunBot();
                command = _botRunner.GetBotCommand();
            }
            catch (TimeLimitExceededException ex)
            {
                Logger.LogException("Bot time limit exceeded ", ex);
                command = new DoNothingCommand();
            }

            if (command.GetType() == typeof (DoNothingCommand))
            {
                _totalDoNothingCommands++;
            }
            else
            {
                _totalDoNothingCommands = 0;
            }

            if (_totalDoNothingCommands == 20)
            {
                PublishCommand(new PlaceBombCommand());
                return;
            }

            WriteLogs();
            PublishCommand(command);
        }

        private void ClearPreviousRoundFiles()
        {
            var dir = Path.Combine(PreviousWorkingDirectory, Settings.Default.StateFileName);

            if(File.Exists(dir))
                File.Delete(dir);

            dir = Path.Combine(PreviousWorkingDirectory, Settings.Default.MapFileName);

            if (File.Exists(dir))
                File.Delete(dir);
        }

        private void RemoveMoveFile()
        {
            var dir = Path.Combine(CurrentWorkingDirectory, Settings.Default.BotMoveFileName);

            if (File.Exists(dir))
                File.Delete(dir);
        }

        private void WriteRoundFiles(GameMap gameMap)
        {
            if (!Directory.Exists(CurrentWorkingDirectory))
                Directory.CreateDirectory(CurrentWorkingDirectory);

            WriteStateFile(gameMap);
            WriteMapFile(gameMap);
        }

        private void WriteStateFile(GameMap gameMap)
        {
            var dir = Path.Combine(CurrentWorkingDirectory, Settings.Default.StateFileName);
            var renderer = new GameMapRender(gameMap, true);
            var json = renderer.RenderJsonGameState();

            CreateFile(dir);

            File.WriteAllText(dir, json.ToString(), new UTF8Encoding(false));
        }

        private void WriteMapFile(GameMap gameMap)
        {
            var dir = Path.Combine(CurrentWorkingDirectory, Settings.Default.MapFileName);
            var renderer = new GameMapRender(gameMap, true);
            var map = renderer.RenderTextGameState();

            CreateFile(dir);

            File.WriteAllText(dir, map.ToString(), new UTF8Encoding(false));
        }

        private void WriteLogs()
        {
            var dir = Path.Combine(CurrentWorkingDirectory, Settings.Default.LogFileName);

            CreateFile(dir);

            File.WriteAllText(dir, _inMemoryLogger.ReadAll(), new UTF8Encoding(false));
        }

        private void CreateFile(string dir)
        {
            if (!File.Exists(dir))
            {
                File.Create(dir).Dispose();
            }
        }

        public string CurrentWorkingDirectory
        {
            get
            {
                var roundPath = Path.Combine(WorkDir, _currentRound.ToString(CultureInfo.InvariantCulture));
                return Path.Combine(roundPath, PlayerEntity.Key.ToString(CultureInfo.InvariantCulture));
            }
        }

        public string PreviousWorkingDirectory
        {
            get
            {
                var roundPath = Path.Combine(WorkDir, (_currentRound - 1).ToString(CultureInfo.InvariantCulture));
                return Path.Combine(roundPath, PlayerEntity.Key.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}
