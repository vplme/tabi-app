using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetQuestionRepository : SqliteNetRepository<Question>, IQuestionRepository
    {
        public SqliteNetQuestionRepository(SQLiteConnection conn) : base(conn)
        {
        }

        public IEnumerable<Question> After(DateTimeOffset begin)
        {
            return connection.Table<Question>()
                            .Where(x => x.Timestamp > begin)
                            .OrderBy(x => x.Timestamp);
        }

        public Question GetLastWithDateTime(string questionKey, DateTimeOffset dateTime)
        {
            DateTimeOffset minDate = new DateTimeOffset(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                0,
                0,
                0,
                0, dateTime.Offset);

            DateTimeOffset maxDate = new DateTimeOffset(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                23,
                59,
                59,
                99, dateTime.Offset);

            return connection.Table<Question>()
                             .Where(x => x.QuestionKey == questionKey && x.QuestionDate >= minDate  && x.QuestionDate <= maxDate)
                             .OrderBy(x => x.Timestamp).LastOrDefault();

        }
    }
}
