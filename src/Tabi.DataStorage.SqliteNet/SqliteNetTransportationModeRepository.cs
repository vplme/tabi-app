using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetTransportationModeRepository : SqliteNetRepository<TransportationMode>, ITransportationModeRepository
    {
        public SqliteNetTransportationModeRepository(SQLiteConnection conn) : base(conn)
        {
        }

        public TransportationMode GetWithChildren(int id)
        {
            throw new NotImplementedException();
        }
    }
}
