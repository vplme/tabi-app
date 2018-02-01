using System;
using System.Collections.Generic;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface ILogEntryRepository : IRepository<LogEntry>
    {
        void ClearLogsBefore(DateTimeOffset before);
        List<LogEntry> After(DateTimeOffset begin);
        int CountBefore(DateTimeOffset dto);
    }
}
