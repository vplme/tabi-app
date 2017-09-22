using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface ITrackEntryRepository : IRepository<TrackEntry>
    {
        TrackEntry LastTrackEntry();
        void ClearAll();
    }
}