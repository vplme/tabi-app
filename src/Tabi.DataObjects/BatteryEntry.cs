using System;
using SQLite;

namespace Tabi.DataObjects
{
    public class BatteryEntry
    {
        [PrimaryKey]
        public DateTimeOffset Timestamp { get; set; }

        public int BatteryLevel { get; set; }

        public BatteryEntryState State { get; set; }
    }

    public enum BatteryEntryState { Discharging = 0, Charging = 1, Full = 2, NotCharging = 3, Unknown = 4 }
}
