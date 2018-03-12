using System;
using System.Collections.Generic;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface IAccelerometerRepository : IRepository<Accelerometer>
    {
        IEnumerable<Accelerometer> GetRange(DateTimeOffset begin, DateTimeOffset end);
    }
}
