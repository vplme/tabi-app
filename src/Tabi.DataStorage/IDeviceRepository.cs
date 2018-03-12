using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface IDeviceRepository : IRepository<Device>
    {
        int Count();
    }
}
