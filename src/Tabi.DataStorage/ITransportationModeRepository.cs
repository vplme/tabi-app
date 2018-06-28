using System;
using System.Collections.Generic;
using System.Text;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface ITransportationModeRepository : IRepository<TransportationModeEntry>
    {
        IEnumerable<TransportationModeEntry> GetRange(DateTimeOffset startTime, DateTimeOffset endTime);
        TransportationModeEntry GetLastWithTrackEntry(int trackId);
    }
}
