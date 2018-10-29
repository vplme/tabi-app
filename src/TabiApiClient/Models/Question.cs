using System;
namespace TabiApiClient.Models
{
    public class Question
    {
        public string Identifier { get; set; }

        public string Answer { get; set; }

        public DateTimeOffset QuestionDate { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}
