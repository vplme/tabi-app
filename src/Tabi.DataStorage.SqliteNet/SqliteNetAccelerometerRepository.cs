using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetAccelerometerRepository : SqliteNetRepository<Accelerometer>, IAccelerometerRepository
    {
        public SqliteNetAccelerometerRepository(SQLiteConnection conn) : base(conn)
        {
            
        }

    }
}
