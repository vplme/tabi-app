using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetHeadingRepository : SqliteNetRepository<Heading>, IHeadingRepository
    {
        public SqliteNetHeadingRepository(SQLiteConnection conn) : base(conn)
        {

        }
    }
}
