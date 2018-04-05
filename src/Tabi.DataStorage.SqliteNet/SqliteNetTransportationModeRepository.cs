using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetTransportationModeRepository : SqliteNetRepository<TransportationModeEntry>, ITransportationModeRepository
    {
        public SqliteNetTransportationModeRepository(SQLiteConnection conn) : base(conn)
        {
        }

        public TransportationModeEntry GetWithChildren(int id)
        {
            throw new NotImplementedException();
        }
    }
}
