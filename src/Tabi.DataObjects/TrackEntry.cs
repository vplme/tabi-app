using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Tabi.DataObjects
{
    public class TrackEntry
    {
        public TrackEntry()
        {
            Id = Guid.NewGuid();
        }

        [PrimaryKey]
        public Guid Id { get; set; }
        
        public int DeviceId { get; set; }

        [Ignore]
        public Device Device { get; set; }
       
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        public double FirstLatitude { get; set; }
        public double FirstLongitude { get; set; }

        public double LastLatitude { get; set; }
        public double LastLongitude { get; set; }

        public TimeSpan TimeTravelled => EndTime - StartTime;

        public double DistanceTravelled { get; set; }
        
        public Guid NextTrackId { get; set; }
        [Ignore]
        public TrackEntry NextTrack { get; set; }
        
        public int NextStopId { get; set; }
        [Ignore]
        public StopVisit NextStop { get; set; }
    }
}