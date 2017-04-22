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

        public IEnumerable<StopVisit> BetweenDates(DateTimeOffset begin, DateTimeOffset end)
        {
            return connection.Table<StopVisit>().Where(x => x.BeginTimestamp >= begin || x.EndTimestamp <= end);
        }

        public StopVisit LastStopVisit()
        { 
            StopVisit visit = connection.Table<StopVisit>().OrderByDescending(sv => sv.EndTimestamp).Last();

            return visit;
        }

        public void Update(StopVisit sv)
        {
            connection.Update(sv);
        }
    }
}
