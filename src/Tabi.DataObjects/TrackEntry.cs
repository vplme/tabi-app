using System;
using SQLite;

namespace Tabi.DataObjects
{
    public class TrackEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public int DeviceId { get; set; }
        [Ignore]
        public Device Device { get; set; }
       
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public TimeSpan TimeTravelled => EndTime - StartTime;

        public double DistanceTravelled { get; set; }
        
        public int NextTrackId { get; set; }
        [Ignore]
        public TrackEntry NextTrack { get; set; }
        
        public int NextStopId { get; set; }
        [Ignore]
        public StopVisit NextStop { get; set; }
    }
}