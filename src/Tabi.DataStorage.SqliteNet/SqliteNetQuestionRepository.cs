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
            return connection.Table<Question>()
                             .Where(x => x.QuestionKey == questionKey && x.QuestionDate >=  dateTime && x.QuestionDate <= dateTime)
                             .OrderBy(x => x.Timestamp).LastOrDefault();

        }
    }
}
