using System;
using SQLite;

namespace Tabi.DataObjects
{
    public class Question
    {
        [Indexed]
        public string QuestionKey { get; set; }

        [Indexed]
        public DateTimeOffset QuestionDate { get; set; }

        public string Answer { get; set; }

        [Indexed]
        public DateTimeOffset Timestamp { get; set; }
    }
}
