using System;
using Microsoft.EntityFrameworkCore;
using Tabi.DataObjects;

namespace Tabi.DataStorage.EFCore
{
    public class EFDeviceRepository : EFRepository<Device>, IDeviceRepository
    {
        public EFDeviceRepository(DbSet<Device> dbSet) : base(dbSet)
        {
        }
    }
}
