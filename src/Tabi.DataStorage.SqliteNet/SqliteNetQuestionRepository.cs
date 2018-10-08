using System;
using System.Collections.Generic;
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
    }
}
