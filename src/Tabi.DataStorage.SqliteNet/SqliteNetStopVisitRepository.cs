using System;
using System.Collections.Generic;
using SQLite;
using System.Linq;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetStopVisitRepository : SqliteNetRepository<StopVisit>, IStopVisitRepository
    {
        public SqliteNetStopVisitRepository(SQLiteConnection conn) : base(conn)
        {
        }

        public IEnumerable<StopVisit> AllSortedByTime()
        {
            return connection.Table<StopVisit>().OrderBy(sv => sv.BeginTimestamp);
        }

        public IEnumerable<StopVisit> BetweenDates(DateTimeOffset begin, DateTimeOffset end)
        {
            return connection.Table<StopVisit>().Where(x => (x.BeginTimestamp >= begin && x.BeginTimestamp <= end) || (x.EndTimestamp >= begin && x.EndTimestamp <= end)).OrderBy(sv => sv.BeginTimestamp);
        }

        public IEnumerable<StopVisit> After(DateTimeOffset begin)
        {
            return connection.Table<StopVisit>()
                             .Where(x => x.EndTimestamp > begin)
                             .OrderBy(x => x.EndTimestamp);
        }

        public void ClearAll()
        {
            connection.DeleteAll<StopVisit>();
        }

        public StopVisit LastStopVisit()
        {
            StopVisit visit = connection.Table<StopVisit>().OrderBy(sv => sv.BeginTimestamp).LastOrDefault();

            return visit;
        }
    }
}
