using System;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetMotiveRepository : SqliteNetRepository<Motive>, IMotiveRepository
    {
        public SqliteNetMotiveRepository(SQLiteConnection conn) : base(conn)
        {
        }
    }
}
