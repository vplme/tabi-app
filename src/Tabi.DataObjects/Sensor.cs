using System;
using System.Collections.Generic;
using System.Text;
using MvvmHelpers;
using Newtonsoft.Json;
using SQLite;

namespace Tabi.DataObjects
{
    public abstract class Sensor : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        public int TrackEntryId { get; set; }

        [Ignore]
        public TrackEntry TrackEntry { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public float Xvalue { get; set; }
        public float Yvalue { get; set; }
        public float Zvalue { get; set; }
    }
}
