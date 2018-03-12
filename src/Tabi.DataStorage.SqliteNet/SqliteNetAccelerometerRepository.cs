using System;
using System.Collections.Generic;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetAccelerometerRepository : SqliteNetRepository<Accelerometer>, IAccelerometerRepository
    {
        public SqliteNetAccelerometerRepository(SQLiteConnection conn) : base(conn)
        {
            
        }
        public IEnumerable<Accelerometer> GetRange(DateTimeOffset begin, DateTimeOffset end)
        {
            try
            {
                return connection.Table<Accelerometer>().Where(x => x.Timestamp >= begin && x.Timestamp <= end);
            }
            catch (Exception e)
            {
                //log error?
                return new List<Accelerometer>();
            }
        }
    }
}
