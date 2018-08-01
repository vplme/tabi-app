using System;
using SQLite;

namespace Tabi.Model
{
    public class Position
    {
        public Position() { }

        [PrimaryKey, Indexed]
        public DateTimeOffset Timestamp { get; set; }

        //public long UnixTimestamp {get {
            //    return Timestamp.
            //}}

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double Altitude { get; set; }

        public double Accuracy { get; set; }

        public double DesiredAccuracy { get; set; }

        public double DistanceBetweenPreviousPosition { get; set; }


        public double Heading { get; set; }
        public double Speed { get; set; }

        public double DistanceTo(Position other)
        {
            return Util.DistanceBetween(Latitude, Longitude, other.Latitude, other.Longitude);
        }

        [Ignore]
        public string Description
        {
            get
            {
                return ToString();
            }
        }
        public override string ToString()
        {
            return String.Format("{0} Long: {1} - Lat: {2} - Accuracy: {3} - Speed {4}", Timestamp, Longitude, Latitude, Accuracy, Speed);
        }
    }
}
