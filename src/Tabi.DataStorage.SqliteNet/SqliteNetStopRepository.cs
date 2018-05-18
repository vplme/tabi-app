using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetStopRepository : SqliteNetRepository<Stop>, IStopRepository
    {
        public SqliteNetStopRepository(SQLiteConnection conn) : base(conn)
        {
        }

        public void Update(Stop s)
        {
            connection.Update(s);
        }

        public List<Stop> After(DateTimeOffset begin) => connection.Table<Stop>()
                             .Where(x => x.Timestamp > begin)
                             .OrderBy(x => x.Timestamp)
                             .ToList();
    }
}
