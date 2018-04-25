﻿using System;
using System.Collections.Generic;
using System.Text;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface ITransportationModeRepository : IRepository<TransportationModeEntry>
    {
        IEnumerable<TransportationModeEntry> GetTransportModesByTrackId(Guid trackId);
        IEnumerable<TransportationModeEntry> GetRange(DateTimeOffset startTime, DateTimeOffset endTime);
    }
}