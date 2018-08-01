using System.Threading.Tasks;

namespace Tabi.DataSync
{
    public interface IDataUploadTask
    {
        Task Start();
    }
}
