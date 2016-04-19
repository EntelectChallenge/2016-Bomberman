using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            var processArgs = String.Format("-Xms512m -jar {0} {1} \"{2}\"", ParentHarness.BotMeta.RunFile,
                ParentHarness.PlayerEntity.Key, ParentHarness.CurrentWorkingDirectory);

            processArgs = AddAdditionalRunArgs(processArgs);


            return new ProcessHandler(ParentHarness.BotDir, Settings.Default.PathToJava, processArgs, ParentHarness.Logger);
        }

        protected override void RunCalibrationTest()
        {
            var calibrationJar = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                @"Calibrations" + Path.DirectorySeparatorChar + "BotCalibrationJava.jar");
            var processArgs = String.Format("-Xms512m -jar {0} {1} \"{2}\"", calibrationJar,
                ParentHarness.PlayerEntity.Key, ParentHarness.CurrentWorkingDirectory);

            using (var handler = new ProcessHandler(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.PathToJava, processArgs, ParentHarness.Logger))
            {
                handler.RunProcess();
            }
        }
    }
}
