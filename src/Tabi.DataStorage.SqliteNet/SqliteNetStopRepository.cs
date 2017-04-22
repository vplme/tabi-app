using System;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetStopRepository : SqliteNetRepository<Stop>, IStopRepository
    {
        public SqliteNetStopRepository(SQLiteConnection conn) : base(conn)
        {
        }
    }
}
