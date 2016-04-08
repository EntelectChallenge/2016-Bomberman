using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Loggers
{
    public class InMemoryLogger : ILogger
    {
        private const String DateFormat = "dd-MM-yyyy HH:mm:ss:fff";
        private StringBuilder _stringBuilder = new StringBuilder();

        public void LogDebug(string message)
        {
            lock (_stringBuilder)
            {
                _stringBuilder.AppendLine().Append(DateTime.Now.ToString(DateFormat)).Append(" - D: \t").Append(message);
            }
        }

        public void LogInfo(string message)
        {
            lock (_stringBuilder)
            {
                _stringBuilder.AppendLine().Append(DateTime.Now.ToString(DateFormat)).Append(" - I: \t").Append(message);
            }
        }

        public void LogException(string message)
        {
            lock (_stringBuilder)
            {
                _stringBuilder.AppendLine().Append(DateTime.Now.ToString(DateFormat)).Append(" - E: \t").Append(message);
            }
        }

        public void LogException(Exception ex)
        {
            lock (_stringBuilder)
            {
                _stringBuilder.AppendLine().Append(DateTime.Now.ToString(DateFormat)).Append(" - E: \t").Append(ex);
            }
        }

        public void LogException(string message, Exception ex)
        {
            lock (_stringBuilder)
            {
                _stringBuilder.AppendLine().Append(DateTime.Now.ToString(DateFormat)).Append(" - E: \t").Append(message).AppendLine(ex.ToString());
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
