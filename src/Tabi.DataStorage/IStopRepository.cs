using System;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface IStopRepository : IRepository<Stop>
    {
        void Update(Stop s);
    }
}
