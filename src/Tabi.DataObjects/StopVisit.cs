using System;
using MvvmHelpers;
using SQLite;

namespace Tabi.DataObjects
{
    public class StopVisit : ObservableObject
    {
        private int id;

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        public int StopId { get; set; }

        private Stop stop;

        [Ignore]
        public Stop Stop
        {
            get => stop;
            set => SetProperty(ref stop, value);
        }

        private DateTimeOffset beginTimestamp;
        public DateTimeOffset BeginTimestamp
        {
            get => beginTimestamp;
            set => SetProperty(ref beginTimestamp, value);
        }

        private DateTimeOffset endTimestamp;
        public DateTimeOffset EndTimestamp
        {
            get => endTimestamp;
            set => SetProperty(ref endTimestamp, value);
        }

        public int NextTrackId { get; set; }
        [Ignore]
        public TrackEntry NextTrack { get; set; }
    }
}
