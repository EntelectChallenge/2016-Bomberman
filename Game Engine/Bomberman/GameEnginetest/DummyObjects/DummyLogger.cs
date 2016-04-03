using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine.Loggers;

namespace GameEnginetest.DummyObjects
{
    public class DummyLogger : ILogger
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
