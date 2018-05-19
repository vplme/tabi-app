using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Tabi.DataObjects;
using Tabi.DataStorage;

namespace Tabi.Shared.DataSync
{
    public class SyncTimeKeeper : ISyncTimeKeeper
    {
        private readonly IRepoManager _repoManager;

        public SyncTimeKeeper(IRepoManager repoManager)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
        }

        public void SetDone(UploadType type, DateTimeOffset timestamp, int count)
        {
            Console.WriteLine($"done {type}: {timestamp} {count}");

            _repoManager.UploadEntryRepository.Add(new UploadEntry() { Type = type, Timestamp = timestamp, Count = count });
        }

        public DateTimeOffset GetPreviousDone(UploadType type)
        {
            DateTimeOffset dto = _repoManager.UploadEntryRepository.GetLastUploadEntry(type).Timestamp;

            Console.WriteLine($"{type}: {dto}");
            return dto;
        }
    }
}
