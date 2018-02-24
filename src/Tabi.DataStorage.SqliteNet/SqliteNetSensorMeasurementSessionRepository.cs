using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetSensorMeasurementSessionRepository : SqliteNetRepository<SensorMeasurementSession>, ISensorMeasurementSession
    {
        public SqliteNetSensorMeasurementSessionRepository(SQLiteConnection conn) : base(conn)
        {
        }
    }
}
