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

        public Motive GetByStopId(int stopId)
        {
            return connection.Table<Motive>().Where(m => m.StopId == stopId).OrderBy(sv => sv.Timestamp).LastOrDefault();
        }

        public IEnumerable<Motive> After(DateTimeOffset begin)
        {
            return connection.Table<Motive>()
                             .Where(x => x.Timestamp > begin)
                             .OrderBy(x => x.Timestamp);
        }

    }
}
