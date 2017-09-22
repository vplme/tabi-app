using System;
using System.Collections.Generic;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface IStopVisitRepository : IRepository<StopVisit>
    {
        IEnumerable<StopVisit> BetweenDates(DateTimeOffset begin, DateTimeOffset end);
        IEnumerable<StopVisit> AllSortedByTime();
        StopVisit LastStopVisit();
        void Update(StopVisit sv);
    }
}
