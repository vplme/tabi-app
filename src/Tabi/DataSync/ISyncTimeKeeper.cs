using System;
using Tabi.DataObjects;

namespace Tabi.DataSync
{
    public interface ISyncTimeKeeper
    {
        void SetDone(UploadType type, DateTimeOffset timestamp, int count);

        DateTimeOffset GetPreviousDone(UploadType type);
    }
}
