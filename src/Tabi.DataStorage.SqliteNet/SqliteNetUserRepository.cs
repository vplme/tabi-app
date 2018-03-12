using System;
using SQLite;
using Tabi.DataObjects;
using Tabi.DataStorage.SqliteNet;

namespace Tabi.DataStorage
{
    public class SqliteNetUserRepository : SqliteNetRepository<User>, IUserRepository
    {
        public SqliteNetUserRepository(SQLiteConnection conn) : base(conn)
        {
        }
    }
}
