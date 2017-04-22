using System;
using SQLite;

namespace Tabi.DataObjects
{
    public class StopVisit
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Ignore]
        public Stop Stop { get; set; }
        public int StopId { get; set; }

        public DateTimeOffset BeginTimestamp { get; set; }
        public DateTimeOffset EndTimestamp { get; set; }


    }
}
