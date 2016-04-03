using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Loggers
{
    public class InMemoryLogger : ILogger
    {
        private StringBuilder _stringBuilder = new StringBuilder();

        public void LogDebug(string message)
        {
            lock (_stringBuilder)
            {
                _stringBuilder.AppendLine().Append("DEBUG: ").Append(message);
            }
        }

        public void LogInfo(string message)
        {
            lock (_stringBuilder)
            {
                _stringBuilder.AppendLine().Append("INFO: ").Append(message);
            }
        }

        public void LogException(string message)
        {
            lock (_stringBuilder)
            {
                _stringBuilder.AppendLine().Append("ERROR: ").Append(message);
            }
        }

        public void LogException(Exception ex)
        {
            lock (_stringBuilder)
            {
                _stringBuilder.AppendLine().Append("ERROR: ").Append(ex);
            }
        }

        public void LogException(string message, Exception ex)
        {
            lock (_stringBuilder)
            {
                _stringBuilder.AppendLine().Append("ERROR: ").Append(message).AppendLine(ex.ToString());
            }
        }

        public string ReadAll()
        {
            lock (_stringBuilder)
            {
                var message = _stringBuilder.ToString();
                _stringBuilder = new StringBuilder();

                return message;
            }
        }
    }
}
