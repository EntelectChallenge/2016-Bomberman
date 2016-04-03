using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Loggers
{
    public class ConsoleLogger : ILogger
    {
        public void LogDebug(string message)
        {
            Console.WriteLine(message);
        }

        public void LogInfo(string message)
        {
            Console.WriteLine(message);
        }

        public void LogException(string message)
        {
            Console.WriteLine(message);
        }

        public void LogException(Exception ex)
        {
            Console.WriteLine(ex);
        }

        public void LogException(string message, Exception ex)
        {
            Console.WriteLine(message);
            Console.WriteLine(ex);
        }

        public string ReadAll()
        {
            return "";
        }
    }
}
