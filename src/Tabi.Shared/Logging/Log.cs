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

        public static LogSeverity SeverityFromString(string s)
        {
            LogSeverity severity;
            switch (s)
            {
                case "fatal":
                    severity = LogSeverity.Fatal;
                    break;
                case "error":
                    severity = LogSeverity.Error;
                    break;
                case "warn":
                    severity = LogSeverity.Warn;
                    break;
                case "info":
                    severity = LogSeverity.Info;
                    break;
                case "debug":
                    severity = LogSeverity.Debug;
                    break;
                case "trace":
                    severity = LogSeverity.Trace;
                    break;
                default:
                    severity = LogSeverity.Debug;
                    break;
            }

            return severity;
        }


    }
}
