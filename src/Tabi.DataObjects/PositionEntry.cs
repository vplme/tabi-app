using System;
using Newtonsoft.Json;
using SQLite;

namespace Tabi.DataObjects
{
    public class PositionEntry
    {
        [PrimaryKey, AutoIncrement, JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public int DeviceId { get; set; }

        [Ignore]
        public Device Device { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Accuracy { get; set; }

        public double Speed { get; set; }
        public double Altitude { get; set; }

        public double DesiredAccuracy { get; set; }
        public double DistanceBetweenPreviousPosition { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public double DistanceTo(PositionEntry other)
        {
            return Util.DistanceBetween(Latitude, Longitude, other.Latitude, other.Longitude);
        }
    }
}