using System;
using System.Collections.Generic;
using System.Text;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface ITransportationModeRepository : IRepository<TransportationMode>
    {
        TransportationMode GetWithChildren(int id);

    }
}
