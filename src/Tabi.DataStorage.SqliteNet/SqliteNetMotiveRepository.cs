using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetMotiveRepository : SqliteNetRepository<Motive>, IMotiveRepository
    {
        public SqliteNetMotiveRepository(SQLiteConnection conn) : base(conn)
        {
        }

        public Motive GetByStopVisitId(int stopVisitId)
        {
            return connection.Table<Motive>().Where(m => m.StopVisitId == stopVisitId).OrderBy(sv => sv.Timestamp).LastOrDefault();
        }

        public IEnumerable<Motive> After(DateTimeOffset begin)
        {
            return connection.Table<Motive>()
                             .Where(x => x.Timestamp > begin)
                             .OrderBy(x => x.Timestamp);
        }

    }
}
