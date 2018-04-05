using System;
using System.Collections.Generic;
using System.Text;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface ISensorRepository<TEntity> : IRepository<TEntity> where TEntity : MotionSensor, new()
    {
        IEnumerable<TEntity> GetRange(DateTimeOffset begin, DateTimeOffset end);
        bool RemoveRangeBeforeTimestamp(DateTimeOffset timestamp);
        bool UpdateTrackKey(TrackEntry trackEntry, string tableName);
    }
}
