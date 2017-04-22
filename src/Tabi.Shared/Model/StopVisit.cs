using System;
using SQLite;

namespace Tabi.Model
{
    public class StopVisit
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int StopId { get; set; }

        [Ignore]
        public Stop Stop { get; set; }

        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }

        public StopVisit()
        {
        }

        public override string ToString()
        {
            return string.Format("[StopVisit: Id={0}, StopId={1}, Stop={2}, Start={3}, End={4}]", Id, StopId, Stop, Start, End);
        }
    }
}
