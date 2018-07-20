using System;
using Newtonsoft.Json;
using SQLite;

namespace Tabi.DataObjects
{
    public class SensorMeasurementSession
    {
        [PrimaryKey, AutoIncrement, JsonIgnore]
        public int Id { get; set; }

        [Indexed]
        public DateTimeOffset Timestamp { get; set; }
        public int AmbientLight { get; set; }
        public int Pedometer { get; set; }
        public bool Proximity{ get; set; }
        public int Compass { get; set; }
        public int BatteryLevel { get; set; }
        public BatteryEntryState BatteryStatus { get; set; }
    }
}
