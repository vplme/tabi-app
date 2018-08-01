using System;
using System.Collections.Generic;

namespace Tabi.Logging
{
    public class MultiLogger : ILogWriter
    {
        private List<ILogWriter> loggers;
        private LogSeverity logLevel;

        public MultiLogger()
        {
            loggers = new List<ILogWriter>();
        }

        public void AddLogger(ILogWriter l)
        {
            loggers.Add(l);
        }

        public void Error(Exception exception)
        {
            foreach (ILogWriter wr in loggers)
            {
                wr.Error(exception);
            }
        }

        public void RemoveLogger(ILogWriter l)
        {
            loggers.Remove(l);
        }

        public void SetLogLevel(LogSeverity severity)
        {
            logLevel = severity;
        }
        public void Write(LogSeverity severity, string str)
        {
            if (severity >= logLevel)
            {
                foreach (ILogWriter wr in loggers)
                {
                    wr.Write(severity, str);
                }
            }
        }
    }
}
