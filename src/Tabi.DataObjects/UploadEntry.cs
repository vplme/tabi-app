using System;
using SQLite;

namespace Tabi.DataObjects
{
    public class UploadEntry
    {
        public int Id { get; set; }

        public UploadType Type { get; set; }

        public int Count { get; set; }

        [Indexed]
        public DateTimeOffset Timestamp { get; set; }
    }

    public enum UploadType
    {
        Attribute,
        BatteryEntry,
        Device,
        LogEntry,
        MotionEntry,
        StopMotive,
        TrackMotive,
        PositionEntry,
        Stop,
        StopVisit,
        TrackEntry,
        TransportationMode,
        Accelerometer,
        Gravity,
        Gyroscope,
        LinearAcceleration,
        Magnetometer,
        MotionSensor,
        Orientation,
        Quaternion,
        SensorMeasurementSession
    }
}
