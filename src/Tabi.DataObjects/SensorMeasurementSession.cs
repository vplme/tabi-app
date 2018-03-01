using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Tabi.DataObjects
{
    public class SensorMeasurementSession
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int TrackEntryId { get; set; }

        [Ignore]
        public TrackEntry TrackEntry { get; set; }

        public DateTimeOffset Timestamp { get; set; }
        public int AmbientLight { get; set; }
        public int Pedometer { get; set; }
        public bool Proximity{ get; set; }
        public int Compass { get; set; }
        public int BatteryLevel { get; set; }
        public BatteryStatus BatteryStatus { get; set; } 
        public PowerSource PowerSource { get; set; }

    }

    public enum PowerSource
    {
        Battery = 1,
        Usb,
        Ac,
        Wireless
    }
    public enum BatteryStatus
    {
        Unknown = 1,
        Charging = 2,
        Discharging = 3,
        NotCharging = 4,
        Full = 5
    }
}
