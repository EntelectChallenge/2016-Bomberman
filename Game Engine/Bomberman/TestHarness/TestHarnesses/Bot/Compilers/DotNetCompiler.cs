using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Domain.Meta;
using GameEngine.Loggers;
using TestHarness.Properties;
using TestHarness.Util;

namespace TestHarness.TestHarnesses.Bot.Compilers
{
    public class DotNetCompiler : ICompiler
    {
        private readonly BotMeta _botMeta;
        private readonly string _botDir;
        private readonly ILogger _compileLogger;

        public DotNetCompiler(BotMeta botMeta, string botDir, ILogger compileLogger)
        {
            _botMeta = botMeta;
            _botDir = botDir;
            _compileLogger = compileLogger;
        }

        public bool HasPackageManager()
        {
            var path = Path.Combine(_botDir, "nuget.exe");
            var exists = File.Exists(path);

            _compileLogger.LogInfo("Checking if bot " + _botMeta.NickName + " has a nuget package manager exe at location " + _botDir);

            return exists;
        }

        public bool RunPackageManager()
        {
            if (!HasPackageManager()) return true;

            _compileLogger.LogInfo("Nuget Package manager Found, running restore");
            using (var handler = new ProcessHandler(_botDir, Path.Combine(_botDir, "nuget.exe"), "restore", _compileLogger))
            {
                handler.ProcessToRun.ErrorDataReceived += ProcessDataRecieved;
                handler.ProcessToRun.OutputDataReceived += ProcessDataRecieved;
                    
                return handler.RunProcess() == 0;
            }
        }

        public bool RunCompiler()
        {
            _compileLogger.LogInfo("Compiling bot " + _botMeta.NickName + " in location " + _botMeta.ProjectLocation + " using .Net");
            using (var handler = new ProcessHandler(Path.Combine(_botDir, _botMeta.ProjectLocation??""), Settings.Default.PathToMSBuild, "/t:rebuild /p:Configuration=Release /p:Platform=\"Any CPU\"", _compileLogger))
            {
                handler.ProcessToRun.ErrorDataReceived += ProcessDataRecieved;
                handler.ProcessToRun.OutputDataReceived += ProcessDataRecieved;

                return handler.RunProcess() == 0;
            }
        }

        void ProcessDataRecieved(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            _compileLogger.LogInfo(e.Data);
        }
    }
}
