using System;

namespace NLogger
{
    public enum LogLevel
    {
        INFO,
        WARNING,
        ERROR
    }

    public interface ILogger
    {
        void Log(string msg, LogLevel level = LogLevel.INFO);
    }

    public static class LoggerTool
    {
        public static string StandardFormat(string msg, LogLevel level = LogLevel.INFO)
        {
            switch (level)
            {
                case LogLevel.INFO:
                    return DateTime.Now.ToLongTimeString() + " - " + msg;
                case LogLevel.WARNING:
                    return DateTime.Now.ToLongTimeString() + " - WARNING WARNING WARNING - " + msg;
                case LogLevel.ERROR:
                    return DateTime.Now.ToLongTimeString() + " - ERROR ERROR ERROR - " + msg;
            }

            throw new NotImplementedException();
        }
    }
}
