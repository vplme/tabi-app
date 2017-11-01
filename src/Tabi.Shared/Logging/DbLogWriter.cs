using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Tabi.DataObjects;

namespace Tabi.Logging
{
    public class DbLogWriter : LogWriter
    {
        LogSeverity severity;
        public DbLogWriter(LogSeverity logLevel = LogSeverity.Info) : base()
        {
            severity = logLevel;
        }


        public override void Write(LogSeverity severity, string str)
        {
            if(severity >= this.severity)
            {
                base.Write(severity, str);
            }
        }

        protected override async Task ConsumerAsync(ISourceBlock<string> Source)
        {
            while (await Source.OutputAvailableAsync())
            {
                LogEntry le = new LogEntry() { Timestamp = DateTimeOffset.Now, Message = Source.Receive() };
                App.RepoManager.LogEntryRepository.Add(le);
            }
        }
    }
}
