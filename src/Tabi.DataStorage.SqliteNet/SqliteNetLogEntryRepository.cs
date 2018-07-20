using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetLogEntryRepository : SqliteNetRepository<LogEntry>, ILogEntryRepository
    {
        public SqliteNetLogEntryRepository(SQLiteConnection conn) : base(conn)
        {
        }

        public void ClearLogsBefore(DateTimeOffset before)
        {
            connection.Execute("DELETE FROM LogEntry WHERE timestamp <= ?", before);
        }

        public IEnumerable<LogEntry> After(DateTimeOffset begin)
        {
            return connection.Table<LogEntry>()
                             .Where(x => x.Timestamp > begin)
                             .OrderBy(x => x.Timestamp);
        }

        public int CountBefore(DateTimeOffset dto)
        {
            return connection.Table<LogEntry>()
                             .Where(x => x.Timestamp <= dto)
                             .Count();
        }

    }
}
