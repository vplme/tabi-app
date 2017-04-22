using System;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetDeviceRepository : SqliteNetRepository<Device>, IDeviceRepository
    {
        public SqliteNetDeviceRepository(SQLiteConnection conn) : base(conn)
        {
        }

        public int Count()
        {
            return connection.Table<Device>().Count();
        }
    }
}

