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
        IEnumerable<StopVisit> After(DateTimeOffset begin);
        IEnumerable<StopVisit> MatchingStop(Stop stop);
        StopVisit First();
        void Update(StopVisit sv);
        void ClearAll();
    }
}
