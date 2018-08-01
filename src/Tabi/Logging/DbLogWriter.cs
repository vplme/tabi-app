using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Tabi.DataObjects;
using Tabi.DataStorage;

namespace Tabi.Logging
{
    public class DbLogWriter : LogWriter
    {
        private readonly IRepoManager _repoManager;
        private LogSeverity _severity;

        public DbLogWriter(IRepoManager repoManager, LogSeverity logLevel = LogSeverity.Info) : base()
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
            _severity = logLevel;
        }

        public override void Write(LogSeverity severity, string str)
        {
            if(severity >= _severity)
            {
                base.Write(severity, str);
            }
        }

        protected override async Task LogConsumerAsync(ISourceBlock<string> Source)
        {
            while (await Source.OutputAvailableAsync())
            {
                LogEntry le = new LogEntry() { Timestamp = DateTimeOffset.Now, Message = Source.Receive() };
                _repoManager.LogEntryRepository.Add(le);
            }
        }

        protected override async Task ErrorConsumerAsync(ISourceBlock<Exception> Source)
        {
            while (await Source.OutputAvailableAsync())
            {
                LogEntry le = new LogEntry() { Event = "Exception", Timestamp = DateTimeOffset.Now, Message = Source.Receive().ToString() };
                _repoManager.LogEntryRepository.Add(le);
            }
        }
    }
}
