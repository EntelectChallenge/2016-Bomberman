using System;
using System.Collections.Generic;
using System.IO;
using Domain.Meta;
using TestHarness.Util;

namespace TestHarness.TestHarnesses.Bot.Runners
{
    public class DotNetRunner : BotRunner
    {
        public DotNetRunner(BotHarness parentHarness) : base(parentHarness)
        {
        }

        protected override ProcessHandler CreateProcessHandler()
        {
            var botDir = ParentHarness.BotDir;
            var botFile = ParentHarness.BotMeta.RunFile; 
            var processName = Path.Combine(botDir, botFile);

            var processArgs = String.Format("{0} \"{1}\"", ParentHarness.PlayerEntity.Key,
                ParentHarness.CurrentWorkingDirectory);
            
            processArgs = AddAdditionalRunArgs(processArgs);

            return new ProcessHandler(botDir, ConvertProcessName(processName), ConvertProcessArgs(processName, processArgs), ParentHarness.Logger, true);
        }

        protected override void RunCalibrationTest()
        {
            var calibrationExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                @"Calibrations" + Path.DirectorySeparatorChar + GetCalibrarionExe());
            var processArgs =
                String.Format("{0} \"{1}\"", ParentHarness.PlayerEntity.Key, ParentHarness.CurrentWorkingDirectory);

			using (var handler = new ProcessHandler(AppDomain.CurrentDomain.BaseDirectory, ConvertProcessName(calibrationExe), ConvertProcessArgs(calibrationExe, processArgs), ParentHarness.Logger, true))
            {
                handler.RunProcess();
            }
        }

        private string ConvertProcessName(string processName)
        {
            return Environment.OSVersion.Platform == PlatformID.Unix ? "mono" : processName;
        }

        private string ConvertProcessArgs(string processName, string args)
        {
            return Environment.OSVersion.Platform == PlatformID.Unix ? String.Format("\"{0}\" {1}", processName, args) : args;
        }

        private string GetCalibrarionExe()
        {
            switch (ParentHarness.BotMeta.BotType)
            {
                case BotMeta.BotTypes.CPlusPlus:
                    return "BotCalibrationCPlusPlus.exe";
                case BotMeta.BotTypes.FSharp:
                    return "BotCalibrationFSharp.exe";
                    case BotMeta.BotTypes.CSharp:
                default:
                        return "BotCalibrationDotNet.exe";
            }
            
        }
    }
}
