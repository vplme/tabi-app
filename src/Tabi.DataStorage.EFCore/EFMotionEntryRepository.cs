using System;
using Microsoft.EntityFrameworkCore;
using Tabi.DataObjects;

namespace Tabi.DataStorage.EFCore
{
    public class EFMotionEntryRepository : EFRepository<MotionEntry>, IMotionEntryRepository
    {
        public EFMotionEntryRepository(DbSet<MotionEntry> dbSet) : base(dbSet)
        {
        }
    }
}
