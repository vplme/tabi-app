using System;
using System.Collections.Generic;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface IBatteryEntryRepository : IRepository<BatteryEntry>
    {
        List<BatteryEntry> After(DateTimeOffset begin);
    }
}
