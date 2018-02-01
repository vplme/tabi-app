using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetBatteryEntryRepository : SqliteNetRepository<BatteryEntry>, IBatteryEntryRepository
    {
        public SqliteNetBatteryEntryRepository(SQLiteConnection conn) : base(conn)
        {
        }

        public List<BatteryEntry> After(DateTimeOffset begin)
        {
            return connection.Table<BatteryEntry>()
                             .Where(x => x.Timestamp > begin)
                             .OrderBy(x => x.Timestamp)
                             .ToList();
        }

        public int CountBefore(DateTimeOffset dto)
        {
            return connection.Table<BatteryEntry>()
                             .Where(x => x.Timestamp <= dto)
                             .Count();
        }
    }
}
