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
    }
}
