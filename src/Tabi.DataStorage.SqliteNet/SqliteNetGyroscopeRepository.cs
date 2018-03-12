using System;
using System.Collections.Generic;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetGyroscopeRepository : SqliteNetRepository<Gyroscope>, IGyroscopeRepository
    {
        public SqliteNetGyroscopeRepository(SQLiteConnection conn) : base(conn)
        {
        }

        public IEnumerable<Gyroscope> GetRange(DateTimeOffset begin, DateTimeOffset end)
        {
            try
            {
                return connection.Table<Gyroscope>().Where(x => x.Timestamp >= begin && x.Timestamp <= end);
            }
            catch (Exception e)
            {
                //log error?
                return new List<Gyroscope>();
            }
        }
    }
}
