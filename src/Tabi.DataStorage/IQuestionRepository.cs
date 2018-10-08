using System;
using System.Collections.Generic;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface IQuestionRepository : IRepository<Question>
    {
        IEnumerable<Question> After(DateTimeOffset begin);
    }
}
