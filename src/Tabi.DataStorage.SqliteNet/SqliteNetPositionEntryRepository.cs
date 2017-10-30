using System;
using System.Collections.Generic;
using SQLite;
using System.Linq;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetPositionEntryRepository : SqliteNetRepository<PositionEntry>, IPositionEntryRepository
    {
        public SqliteNetPositionEntryRepository(SQLiteConnection conn) : base(conn)
        {
        }

        public List<PositionEntry> FilterAccuracy(double accuracy)
        {
            return connection.Table<PositionEntry>().Where(p => p.Accuracy <= 100).ToList();
        }

        public List<PositionEntry> FilterPeriodAccuracy(DateTimeOffset begin, DateTimeOffset end, double accuracy)
        {
            return connection.Table<PositionEntry>()
                             .Where(x => x.Timestamp >= begin && x.Timestamp <= end)
                             .Where(p => p.Accuracy <= accuracy)
                             .OrderBy(x => x.Timestamp)
                             .ToList();
        }

        public List<PositionEntry> After(DateTimeOffset begin)
        {
            return connection.Table<PositionEntry>()
                             .Where(x => x.Timestamp >= begin)
                             .OrderBy(x => x.Timestamp)
                             .ToList();
        }

        public List<PositionEntry> TakeLast(int count)
        {
            return connection.Table<PositionEntry>().OrderByDescending(x => x.Timestamp).Take(count).ToList();
        }
    }
}
