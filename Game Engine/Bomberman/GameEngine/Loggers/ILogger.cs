using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Loggers
{
    public interface ILogger
    {
        void LogDebug(String message);
        void LogInfo(String message);
        void LogException(String message);
        void LogException(Exception ex);
        void LogException(String message, Exception ex);
        String ReadAll();
    }
}
