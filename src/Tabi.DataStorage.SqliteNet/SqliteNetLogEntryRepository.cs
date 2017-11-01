using System;
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
    }
}
