using System;
using MvvmHelpers;
using Newtonsoft.Json;
using SQLite;

namespace Tabi.DataObjects
{
    public class StopVisit : ObservableObject
    {
        private int id;

        [PrimaryKey, AutoIncrement, JsonProperty("PhoneStopVisitId")]
        public int Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        [JsonIgnore]
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

        public double StopAccuracy { get; set; }

        private double latitude;

        public double Latitude { get => latitude; set => SetProperty(ref latitude, value); }

        private double longitude;

        public double Longitude { get => longitude; set => SetProperty(ref longitude, value); }

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
