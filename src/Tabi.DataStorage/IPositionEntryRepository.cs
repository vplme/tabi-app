using System;
using System.Collections.Generic;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface IPositionEntryRepository : IRepository<PositionEntry>
    {
        List<PositionEntry> TakeLast(int count);
        List<PositionEntry> FilterAccuracy(double accuracy);
        List<PositionEntry> FilterPeriodAccuracy(DateTimeOffset begin, DateTimeOffset end, double accuracy);
        IEnumerable<PositionEntry> After(DateTimeOffset begin);
        int CountBefore(DateTimeOffset dto);
    }
}
