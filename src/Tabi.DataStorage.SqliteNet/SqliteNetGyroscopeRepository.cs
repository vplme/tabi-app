using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetGyroscopeRepository : SqliteNetRepository<Gyroscope>, IGyroscopeRepository
    {
        public SqliteNetGyroscopeRepository(SQLiteConnection conn) : base(conn)
        {
        }
    }
}
