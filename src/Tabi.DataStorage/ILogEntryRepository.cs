using System;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface ILogEntryRepository : IRepository<LogEntry>
    {
        void ClearLogsBefore(DateTimeOffset before);
    }
}
