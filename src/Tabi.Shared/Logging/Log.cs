using System;
using Tabi.Logging;

namespace Tabi
{
    public static class Log
    {
        private static ILogWriter logger;

        private static void LogWrite(LogSeverity severity, string text)
        {
            logger?.Write(severity, text);
        }

        public static void SetLogger(ILogWriter l)
        {
            logger = l;
        }

        public static void Trace(string text)
        {
            LogWrite(LogSeverity.Trace, text);
        }

        public static void Debug(string text)
        {
            LogWrite(LogSeverity.Debug, text);
        }

        public static void Info(string text)
        {
            LogWrite(LogSeverity.Info, text);
        }

        public static void Warn(string text)
        {
            LogWrite(LogSeverity.Warn, text);
        }

        public static void Error(string text)
        {
            LogWrite(LogSeverity.Error, text);
        }

        public static void Fatal(string text)
        {
            LogWrite(LogSeverity.Fatal, text);
        }



    }
}
