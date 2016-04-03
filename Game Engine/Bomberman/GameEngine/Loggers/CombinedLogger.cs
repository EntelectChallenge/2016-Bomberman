using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Loggers
{
    public class CombinedLogger : ILogger
    {
        private readonly List<ILogger> _loggers;

        public CombinedLogger(params ILogger[] loggers)
        {
            _loggers = new List<ILogger>(loggers);
        }

        public void LogDebug(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.LogDebug(message);
            }
        }

        public void LogInfo(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.LogInfo(message);
            }
        }

        public void LogException(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.LogException(message);
            }
        }

        public void LogException(Exception ex)
        {
            foreach (var logger in _loggers)
            {
                logger.LogException(ex);
            }
        }

        public void LogException(string message, Exception ex)
        {
            foreach (var logger in _loggers)
            {
                logger.LogException(message, ex);
            }
        }

        public string ReadAll()
        {
            return _loggers.Aggregate("", (current, logger) => current + logger.ReadAll());
        }
    }
}
