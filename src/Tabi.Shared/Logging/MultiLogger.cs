using System;
using System.Collections.Generic;

namespace Tabi.Logging
{
    public class MultiLogger : ILogWriter
    {
        private List<ILogWriter> loggers;

        public MultiLogger()
        {
            loggers = new List<ILogWriter>();
        }

        public void AddLogger(ILogWriter l)
        {
            loggers.Add(l);
        }

        public void RemoveLogger(ILogWriter l)
        {
            loggers.Remove(l);
        }

        public void Write(string str)
        {
            foreach(ILogWriter wr in loggers)
            {
                wr.Write(str);
            }
        }
    }
}
