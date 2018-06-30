using System;
using System.Threading.Tasks;

namespace Tabi.Shared.DataSync
{
    public interface IDataUploadTask
    {
        Task Start();
    }
}
