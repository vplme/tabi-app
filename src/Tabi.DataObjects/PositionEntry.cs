using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SQLite;

namespace Tabi.DataObjects
{
    public class PositionEntry
    {
        [PrimaryKey, AutoIncrement, JsonIgnore]
        public int Id { get; set; }
        
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Accuracy { get; set; }

        public double Speed { get; set; }
        public double Course { get; set; }

        public double Altitude { get; set; }
        public double VerticalAccuracy { get; set; }

        public double DesiredAccuracy { get; set; }
        public double DistanceBetweenPreviousPosition { get; set; }

        public string Comment { get; set; }

        [Indexed]
        public DateTimeOffset Timestamp { get; set; }

        public double DistanceTo(PositionEntry other)
        {
            return Util.DistanceBetween(Latitude, Longitude, other.Latitude, other.Longitude);
        }

        public override bool Equals(object obj)
        {
            var entry = obj as PositionEntry;
            return entry != null &&
                   Latitude == entry.Latitude &&
                   Longitude == entry.Longitude &&
                   Accuracy == entry.Accuracy &&
                   Speed == entry.Speed &&
                   Course == entry.Course &&
                   Altitude == entry.Altitude &&
                   VerticalAccuracy == entry.VerticalAccuracy &&
                   Timestamp.Equals(entry.Timestamp);
        }

        public override int GetHashCode()
        {
            var hashCode = -681162261;
            hashCode = hashCode * -1521134295 + Latitude.GetHashCode();
            hashCode = hashCode * -1521134295 + Longitude.GetHashCode();
            hashCode = hashCode * -1521134295 + Accuracy.GetHashCode();
            hashCode = hashCode * -1521134295 + Speed.GetHashCode();
            hashCode = hashCode * -1521134295 + Course.GetHashCode();
            hashCode = hashCode * -1521134295 + Altitude.GetHashCode();
            hashCode = hashCode * -1521134295 + VerticalAccuracy.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<DateTimeOffset>.Default.GetHashCode(Timestamp);
            return hashCode;
        }
    }
}