using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TestHarness.Properties;
using TestHarness.Util;

namespace TestHarness.TestHarnesses.Bot.Runners
{
    public class JavaScriptRunner : BotRunner
    {
        public JavaScriptRunner(BotHarness parentHarness) : base(parentHarness)
        {
        }

        protected override ProcessHandler CreateProcessHandler()
        {
            var processArgs = String.Format("{0} {1} \"{2}\"", ParentHarness.BotMeta.RunFile, ParentHarness.PlayerEntity.Key,
                ParentHarness.CurrentWorkingDirectory);

            processArgs = AddAdditionalRunArgs(processArgs);

            return new ProcessHandler(ParentHarness.BotDir, Settings.Default.PathToNode, processArgs, ParentHarness.Logger);
        }

        protected override void RunCalibrationTest()
        {
            var calibrationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
               @"Calibrations" + Path.DirectorySeparatorChar + "BotCalibrationNode.js");
            var processArgs = String.Format("{0} {1} \"{2}\"", calibrationFile,
                ParentHarness.PlayerEntity.Key, ParentHarness.CurrentWorkingDirectory);

            using (var handler = new ProcessHandler(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.PathToNode, processArgs, ParentHarness.Logger))
            {
                handler.RunProcess();
            }
        }
    }
}
