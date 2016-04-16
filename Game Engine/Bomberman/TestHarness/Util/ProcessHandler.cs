using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using GameEngine.Loggers;
using TestHarness.Properties;

namespace TestHarness.Util
{
    public class ProcessHandler : IDisposable
    {
        private readonly Process _processToRun;
        private readonly ILogger _logger;

        public bool LimitExecutionTime { get; set; }

        public ProcessHandler(string workDir, string processName, string processArgs, ILogger logger) :
		this(workDir, processName, processArgs, logger, false)
        {
        }

        public ProcessHandler(string workDir, string processName, string processArgs, ILogger logger, bool isMono)
        {
            _logger = logger;
            _processToRun = CreateProcess(workDir, processName, processArgs, isMono);
            LimitExecutionTime = false;
        }

        public Process ProcessToRun
        {
            get { return _processToRun; }
        }

        private Process CreateProcess(string workDir, string processName, string processArgs, bool isMono)
        {
            var process = new Process()
            {
                StartInfo =
                {
                    WorkingDirectory = workDir,
                    FileName = processName,
                    Arguments = processArgs,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    ErrorDialog = false,
                    StandardErrorEncoding = Encoding.UTF8,
                    StandardOutputEncoding = Encoding.UTF8
                }
            };

            return process;
        }

        public int RunProcess()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                return StartProcessCommon();
            }
            
            using (
                var newErrorMode =
                    new ChangeErrorMode(ChangeErrorMode.ErrorModes.FailCriticalErrors |
                                        ChangeErrorMode.ErrorModes.NoGpFaultErrorBox))
            {
                return StartProcessCommon();
            }
        }

        private int StartProcessCommon()
        {
            _logger.LogInfo("Executing process " + _processToRun.StartInfo.FileName + " " + _processToRun.StartInfo.Arguments);
            _processToRun.EnableRaisingEvents = true;
            _processToRun.Start();
            _processToRun.BeginOutputReadLine();
            _processToRun.BeginErrorReadLine();
            if (Environment.OSVersion.Platform != PlatformID.Unix) {
                // On Linux root permissions are required to change the PriorityClass
                _processToRun.PriorityClass = ProcessPriorityClass.AboveNormal;
            }

            var cleanExit = true;
            if (LimitExecutionTime)
            {
                _logger.LogInfo("Bot has " + (TimeSpan.FromSeconds(Settings.Default.MaxBotRuntimeSeconds * 2).TotalMilliseconds) + "ms to run before it will be forcefully killed");
                cleanExit = _processToRun.WaitForExit((int) (TimeSpan.FromSeconds(Settings.Default.MaxBotRuntimeSeconds * 2).TotalMilliseconds));
            }

            if (!cleanExit)
            {
                _logger.LogInfo("Bot has been killed for taking to long to execute");
                _processToRun.Kill();
            }
            //Ensure that all output events have been written before resuming with the main thread
            _processToRun.WaitForExit();

            return _processToRun.ExitCode;
        }

        public void Dispose()
        {
            if(_processToRun != null)
                _processToRun.Dispose();
        }
    }
}
