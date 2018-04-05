using System;
using System.Collections.Generic;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface ITrackEntryRepository : IRepository<TrackEntry>
    {
        TrackEntry GetWithChildren(Guid id);
        TrackEntry LastTrackEntry();
        bool UpdateWithChildren(TrackEntry track);
        TrackEntry LastCompletedTrackEntry();
        IEnumerable<TrackEntry> GetRangeByEndTime(DateTimeOffset begin, DateTimeOffset end);
        void ClearAll();
    }
}