using System;
using System.Collections.Generic;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface IStopRepository : IRepository<Stop>
    {
        void Update(Stop s);
        IEnumerable<Stop> After(DateTimeOffset begin);
        IEnumerable<Stop> NearestStops(double latitude, double longitude, double radius);
    }
}
