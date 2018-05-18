using System;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface IUploadEntryRepository : IRepository<UploadEntry>
    {
        UploadEntry GetLastUploadEntry(UploadType type);
    }
}
