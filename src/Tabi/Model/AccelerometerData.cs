using System;

namespace Tabi.Model

{
    public class AccelerometerData
    {
        public AccelerometerData()
        {
        }
        public AccelerometerType Type { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public DateTimeOffset Timestamp { get; set; }


        public override string ToString()
        {
            return string.Format("[AccelerometerData: Type={0}, X={1}, Y={2}, Z={3}, Timestamp={4}]", Type, X, Y, Z, Timestamp);
        }
    }

    public enum AccelerometerType
    {
        Total = 1,
        Gravity = 2,
        User = 3,
    }


}
