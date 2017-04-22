using System;
using Microsoft.EntityFrameworkCore;

namespace Tabi.DataStorage.EFCore
{
    public class RepoManager : IRepoManager
    {
        static bool initialized;
        static string path;
        static RepoManager instance;

        TabiContext context;

        public RepoManager()
        {
            context = new TabiContext(path);
            context.Database.Migrate();
            context.Database.EnsureCreated();

            UserRepository = new EFUserRepository(context.Users);
            DeviceRepository = new EFDeviceRepository(context.Devices);
            MotionEntryRepository = new EFMotionEntryRepository(context.MotionEntries);
            PositionEntryRepository = new EFPositionEntryRepository(context.PositionEntries);
        }

        public static void Setup(string path)
        {
            initialized = true;
            RepoManager.path = path;
        }

        public IUserRepository UserRepository { get; private set; }
        public IDeviceRepository DeviceRepository { get; private set; }
        public IPositionEntryRepository PositionEntryRepository { get; private set; }
        public IMotionEntryRepository MotionEntryRepository { get; private set; }

        public static RepoManager GetInstance()
        {
            if (!initialized)
            {
                throw new InvalidOperationException("Manager must be setup with path.");
            }

            if (instance == null)
            {
                instance = new RepoManager();
            }

            return instance;
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}