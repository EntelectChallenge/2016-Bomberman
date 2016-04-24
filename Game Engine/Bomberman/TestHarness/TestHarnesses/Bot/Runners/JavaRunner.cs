using System;
using System.IO;
using TestHarness.Properties;
using TestHarness.Util;

namespace TestHarness.TestHarnesses.Bot.Runners
{
    public class JavaRunner : BotRunner
    {
        public JavaRunner(BotHarness parentHarness) : base(parentHarness)
        {
        }

        protected override ProcessHandler CreateProcessHandler()
        {
            var processArgs = GetProcessArguments(ParentHarness.BotMeta.RunFile, ParentHarness.PlayerEntity.Key, ParentHarness.CurrentWorkingDirectory);
            processArgs = AddAdditionalRunArgs(processArgs);

            return new ProcessHandler(ParentHarness.BotDir, Settings.Default.PathToJava, processArgs, ParentHarness.Logger);
        }

        protected override void RunCalibrationTest()
        {
            var calibrationJar = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                @"Calibrations" + Path.DirectorySeparatorChar + "BotCalibrationJava.jar");
            var processArgs = GetProcessArguments(calibrationJar, ParentHarness.PlayerEntity.Key, ParentHarness.CurrentWorkingDirectory);

            using (var handler = new ProcessHandler(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.PathToJava, processArgs, ParentHarness.Logger))
            {
                handler.RunProcess();
            }
        }


        private static string GetProcessArguments(string jarFilePath, char playerKey, string workingDirectory)
        {
            return String.Format("-Xms512m -jar \"{0}\" {1} \"{2}\"", jarFilePath, playerKey, workingDirectory);
        }
    }
}
