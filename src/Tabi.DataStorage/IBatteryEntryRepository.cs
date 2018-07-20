using System;
using System.Collections.Generic;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface IBatteryEntryRepository : IRepository<BatteryEntry>
    {
        IEnumerable<BatteryEntry> After(DateTimeOffset begin);
        int CountBefore(DateTimeOffset dto);
    }
}
