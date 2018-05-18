using System;
using System.Collections.Generic;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface IStopRepository : IRepository<Stop>
    {
        void Update(Stop s);
        List<Stop> After(DateTimeOffset begin);
    }
}
