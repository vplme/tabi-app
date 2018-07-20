using System;
using MvvmHelpers;
using Newtonsoft.Json;
using SQLite;

namespace Tabi.DataObjects
{
    public abstract class MotionSensor : ObservableObject
    {
        [PrimaryKey, AutoIncrement, JsonIgnore]
        public long Id { get; set; }

        [Indexed]
        public DateTimeOffset Timestamp { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
}
