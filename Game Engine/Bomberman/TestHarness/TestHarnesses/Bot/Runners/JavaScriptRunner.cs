using System;
using System.Collections.Generic;
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
            return new ProcessHandler(ParentHarness.BotDir, Settings.Default.PathToNode, processArgs, ParentHarness.Logger);
        }

        protected override void RunCalibrationTest()
        {
        }
    }
}
