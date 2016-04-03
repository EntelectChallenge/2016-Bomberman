using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using Domain.Meta;
using GameEngine.Commands;
using GameEngine.Commands.PlayerCommands;
using TestHarness.Exceptions;
using TestHarness.Properties;
using TestHarness.Util;

namespace TestHarness.TestHarnesses.Bot
{
    public abstract class BotRunner
    {
        protected readonly BotHarness ParentHarness;
        protected TimeSpan MaxRunTime;
        private int _botReturnCode;

        protected BotRunner(BotHarness parentHarness)
        {
            ParentHarness = parentHarness;

            MaxRunTime = TimeSpan.FromSeconds(Settings.Default.MaxBotRuntimeSeconds);
        }

        public void CalibrateBot()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            RunCalibrationTest();
            stopWatch.Stop();

            MaxRunTime = MaxRunTime.Add(stopWatch.Elapsed);

            ParentHarness.Logger.LogInfo(String.Format("Bot calibration complete and can run an additional {0}ms due to environment startup", stopWatch.ElapsedMilliseconds));
        }

        public void RunBot()
        {
            using (var handler = CreateProcessHandler())
            {
                try
                {
                    handler.LimitExecutionTime = true;
                    handler.ProcessToRun.OutputDataReceived += (sender, args) => ParentHarness.Logger.LogInfo("Output from bot: " + args.Data);
                    handler.ProcessToRun.ErrorDataReceived += (sender, args) => ParentHarness.Logger.LogException("Output from bot: " + args.Data);
                    ParentHarness.Logger.LogDebug(String.Format("Executing bot with following commands {0} {1}", handler.ProcessToRun.StartInfo.FileName, handler.ProcessToRun.StartInfo.Arguments));
                    _botReturnCode = handler.RunProcess();

                    ParentHarness.Logger.LogInfo("Your bots total processor time was " + handler.ProcessToRun.TotalProcessorTime);
                }
                catch (Exception ex)
                {
                    ParentHarness.Logger.LogException("Failure while executing bot " + handler.ProcessToRun.StartInfo.FileName + " " + handler.ProcessToRun.StartInfo.Arguments, ex);
                    _botReturnCode = -1;
                }

                if (handler.ProcessToRun.TotalProcessorTime >= MaxRunTime)
                {
                    ParentHarness.Logger.LogInfo("Your bot exceeded the maximum processor time");
                    throw new TimeLimitExceededException("Time limit exceeded by " + (handler.ProcessToRun.TotalProcessorTime - MaxRunTime) + " milliseconds");
                }
            }
        }

        public ICommand GetBotCommand()
        {
            try
            {
                var commandCode = GetBotCommandFromFile();

                switch (commandCode)
                {
                    case 1:
                        return new MovementCommand(MovementCommand.Direction.Up);
                    case 2:
                        return new MovementCommand(MovementCommand.Direction.Left);
                    case 3:
                        return new MovementCommand(MovementCommand.Direction.Right);
                    case 4:
                        return new MovementCommand(MovementCommand.Direction.Down);
                    case 5:
                        return new PlaceBombCommand();
                    case 6:
                        return new TriggerBombCommand();
                    default:
                        return new DoNothingCommand();
                }
            }
            catch (Exception ex)
            {
                ParentHarness.Logger.LogException("Failed to read move file from bot", ex);
                return new DoNothingCommand();
            }
        }

        private int GetBotCommandFromFile()
        {
            var workDir = ParentHarness.CurrentWorkingDirectory;
            var moveFile = Path.Combine(workDir, Settings.Default.BotMoveFileName);

            if (!File.Exists(moveFile))
                return 0;

            var code = File.ReadAllText(moveFile).FirstOrDefault();

            return Char.IsNumber(code) ? (int)Char.GetNumericValue(code) : 0;
        }

        protected abstract ProcessHandler CreateProcessHandler();
        protected abstract void RunCalibrationTest();
    }
}
