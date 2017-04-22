using System;
using SQLite;

namespace Tabi.DataObjects
{
    public enum MotionType { Linear = 0 }

    public class MotionEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public Guid DeviceId { get; set; }

        [Ignore]
        public Device Device { get; set; }

        public MotionType Type { get; set; }

        public double Value1 { get; set; }

        public double Value2 { get; set; }

        public double Value3 { get; set; }
    }
}