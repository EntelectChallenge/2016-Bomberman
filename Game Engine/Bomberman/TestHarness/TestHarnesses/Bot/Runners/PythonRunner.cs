using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Meta;
using TestHarness.Properties;
using TestHarness.Util;

namespace TestHarness.TestHarnesses.Bot.Runners
{
    public class PythonRunner : BotRunner
    {
        public PythonRunner(BotHarness parentHarness)
            : base(parentHarness)
        {
        }

        protected override ProcessHandler CreateProcessHandler()
        {
            var pythonExecutable = Settings.Default.PathToPython3;
            if (ParentHarness.BotMeta.BotType == BotMeta.BotTypes.Python2)
            {
                pythonExecutable = Settings.Default.PathToPython2;
            }

            var processArgs = String.Format("{0} {1} \"{2}\"", ParentHarness.BotMeta.RunFile,
                ParentHarness.PlayerEntity.Key, ParentHarness.CurrentWorkingDirectory);


            return new ProcessHandler(ParentHarness.BotDir, pythonExecutable, processArgs, ParentHarness.Logger);
        }

        protected override void RunCalibrationTest()
        {
        }
    }
}
