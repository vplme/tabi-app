using System;
using System.Collections.Generic;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface IGyroscopeRepository : IRepository<Gyroscope>
    {
        IEnumerable<Gyroscope> GetRange(DateTimeOffset begin, DateTimeOffset end);
    }
}
