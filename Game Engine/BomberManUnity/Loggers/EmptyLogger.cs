using System;
using System.Collections.Generic;
using System.Text;
using GameEngine.Loggers;

namespace BomberManUnity.Loggers
{
    public class EmptyLogger : ILogger
    {
        public void LogDebug(string message)
        {
        }

        public void LogInfo(string message)
        {
        }

        public void LogException(string message)
        {
        }

        public void LogException(Exception ex)
        {
        }

        public void LogException(string message, Exception ex)
        {
        }

        public string ReadAll()
        {
            return "";
        }
    }
}
