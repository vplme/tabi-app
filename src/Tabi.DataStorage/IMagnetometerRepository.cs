using System;
using System.Collections.Generic;
using System.Text;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface IMagnetometerRepository : IRepository<Magnetometer>
    {
        IEnumerable<Magnetometer> GetRange(DateTimeOffset begin, DateTimeOffset end);
    }
}
