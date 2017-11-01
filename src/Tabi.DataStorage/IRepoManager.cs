using System;
namespace Tabi.DataStorage
{
    public interface IRepoManager
    {
        IUserRepository UserRepository { get; }
        IDeviceRepository DeviceRepository { get; }
        IPositionEntryRepository PositionEntryRepository { get; }
        IMotionEntryRepository MotionEntryRepository { get; }
        IStopRepository StopRepository { get; }
        IStopVisitRepository StopVisitRepository { get; }
        IBatteryEntryRepository BatteryEntryRepository { get; }
        ITrackEntryRepository TrackEntryRepository { get; }
        ILogEntryRepository LogEntryRepository { get; }

        void SaveChanges();
    }
}
