using System;
using Moq;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Shared.DataSync;
using Xunit;

namespace Tabi.Test
{
    public class TimeKeeperTest
    {
        [Fact]
        public void TimeKeeperMockStoresTime()
        {
            Mock<IRepoManager> mockRepo = new Mock<IRepoManager>();
            IUploadEntryRepository ueRep = new MockUploadEntryRepository();

            mockRepo.Setup(r => r.UploadEntryRepository).Returns(ueRep);

            IRepoManager repoManager = mockRepo.Object;

            ISyncTimeKeeper tk = new SyncTimeKeeper(repoManager);
            DateTimeOffset now = DateTimeOffset.Now;

            tk.SetDone(UploadType.Attribute, now, 10);

            DateTimeOffset previousDone = tk.GetPreviousDone(UploadType.Attribute);

            Assert.Equal(now, previousDone);
        }


        [Fact]
        public void TimeKeeperSetDone()
        {
            Mock<IRepoManager> mockRepo = new Mock<IRepoManager>();
            IRepoManager repoManager = mockRepo.Object;

            Mock<IUploadEntryRepository> ueRep = new Mock<IUploadEntryRepository>();

            UploadEntry addedUploadEntry = null;
            mockRepo.Setup(x => x.UploadEntryRepository).Returns(ueRep.Object);
            ueRep.Setup(x => x.Add(It.IsAny<UploadEntry>())).Callback((UploadEntry ue) => addedUploadEntry = ue);


            ISyncTimeKeeper tk = new SyncTimeKeeper(repoManager);
            DateTimeOffset now = DateTimeOffset.Now;

            tk.SetDone(UploadType.Attribute, now, 10);


            mockRepo.Verify(x => x.UploadEntryRepository);
            ueRep.Verify(x => x.Add(It.IsAny<UploadEntry>()));

            Assert.Equal(10, addedUploadEntry.Count);
            Assert.Equal(now, addedUploadEntry.Timestamp);
            Assert.Equal(UploadType.Attribute, addedUploadEntry.Type);
        }


        [Fact]
        public void TimeKeeperGetPreviousDone()
        {
            Mock<IRepoManager> mockRepo = new Mock<IRepoManager>();
            IRepoManager repoManager = mockRepo.Object;

            Mock<IUploadEntryRepository> ueRep = new Mock<IUploadEntryRepository>();

            UploadEntry resultUploadEntry = new UploadEntry
            {
                Count = 10,
                Timestamp = DateTimeOffset.Now,
                Type = UploadType.Attribute,
            };

            mockRepo.Setup(x => x.UploadEntryRepository).Returns(ueRep.Object);
            ueRep.Setup(x => x.GetLastUploadEntry(UploadType.Attribute)).Returns(resultUploadEntry);
            ISyncTimeKeeper tk = new SyncTimeKeeper(repoManager);


            DateTimeOffset resultDto = tk.GetPreviousDone(UploadType.Attribute);


            mockRepo.Verify(x => x.UploadEntryRepository);
            ueRep.Verify(x => x.GetLastUploadEntry(UploadType.Attribute));
            Assert.Equal(resultUploadEntry.Timestamp, resultDto);
        }
    }
}
