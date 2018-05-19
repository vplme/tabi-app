using System;
using SQLite;

namespace Tabi.DataObjects
{
    public class Motive
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int StopId { get; set; }

        public int TrackId { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string Text { get; set; }
    }
}
