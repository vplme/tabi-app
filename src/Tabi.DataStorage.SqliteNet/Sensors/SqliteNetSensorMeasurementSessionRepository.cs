using System;
using System.Collections.Generic;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetSensorMeasurementSessionRepository : SqliteNetRepository<SensorMeasurementSession>, ISensorMeasurementSessionRepository
    {
        public SqliteNetSensorMeasurementSessionRepository(SQLiteConnection conn) : base(conn)
        {

        }

        public IEnumerable<SensorMeasurementSession> GetRange(DateTimeOffset begin, DateTimeOffset end)
        {
            try
            {
                return connection.Table<SensorMeasurementSession>().Where(x => x.Timestamp >= begin && x.Timestamp <= end);            
            }
            catch (Exception e)
            {
                //log error?
                return new List<SensorMeasurementSession>();
            }
        }
    }
}
