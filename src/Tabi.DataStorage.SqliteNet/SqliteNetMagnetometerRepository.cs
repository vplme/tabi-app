using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetMagnetometerRepository : SqliteNetRepository<Magnetometer>, IMagnetometerRepository
    {
        public SqliteNetMagnetometerRepository(SQLiteConnection conn) : base(conn)
        {
        }
        public IEnumerable<Magnetometer> GetRange(DateTimeOffset begin, DateTimeOffset end)
        {
            try
            {
                return connection.Table<Magnetometer>().Where(x => x.Timestamp >= begin && x.Timestamp <= end);
            }
            catch (Exception e)
            {
                //log error?
                return new List<Magnetometer>();
            }
        }
    }
}
