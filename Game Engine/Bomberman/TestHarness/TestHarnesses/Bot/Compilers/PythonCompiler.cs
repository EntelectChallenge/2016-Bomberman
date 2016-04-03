using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Domain.Meta;
using GameEngine.Loggers;
using TestHarness.Properties;
using TestHarness.Util;

namespace TestHarness.TestHarnesses.Bot.Compilers
{
    public class PythonCompiler : ICompiler
    {
        private readonly BotMeta _botMeta;
        private readonly string _botDir;
        private readonly ILogger _compileLogger;

        public PythonCompiler(BotMeta botMeta, string botDir, ILogger compileLogger)
        {
            _botMeta = botMeta;
            _botDir = botDir;
            _compileLogger = compileLogger;
        }

        public bool HasPackageManager()
        {
            var path = Path.Combine(_botDir, "requirements.txt");
            var exists = File.Exists(path);

            _compileLogger.LogInfo("Checking if bot " + _botMeta.NickName + " has a requirements.txt file " + _botDir);

            return exists;
        }

        public bool RunPackageManager()
        {
            if (!HasPackageManager()) return true;

            _compileLogger.LogInfo("Found requirements.txt, doing install");
            using (var handler = new ProcessHandler(_botDir, Settings.Default.PathToPythonPackageIndex, "install -r requirements.txt", _compileLogger))
            {
                handler.ProcessToRun.ErrorDataReceived += ProcessDataRecieved;
                handler.ProcessToRun.OutputDataReceived += ProcessDataRecieved;

                return handler.RunProcess() == 0;
            }
        }

        public bool RunCompiler()
        {
            _compileLogger.LogInfo("Compiling bot " + _botMeta.NickName + " using python");
            return true;
        }

        void ProcessDataRecieved(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            _compileLogger.LogInfo(e.Data);
        }
    }
}
