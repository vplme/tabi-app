using System;
using SQLite;

namespace Tabi.DataObjects
{
    public class Question
    {
        public string QuestionKey { get; set; }

        public string Answer { get; set; }

        [Indexed]
        public DateTimeOffset Timestamp { get; set; }
    }
}
