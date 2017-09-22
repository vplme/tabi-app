using System;
using SQLite;

namespace Tabi.DataObjects
{
    public class StopVisit
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int StopId { get; set; }
        [Ignore]
        public Stop Stop { get; set; }
        
        public int DeviceId { get; set; }
        [Ignore]
        public Device Device { get; set; }

        public DateTimeOffset BeginTimestamp { get; set; }
        public DateTimeOffset EndTimestamp { get; set; }

        public int NextTrackId { get; set; }
        [Ignore]
        public TrackEntry NextTrack { get; set; }
    }
}
