using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tabi.DataObjects;

namespace Tabi.DataStorage.EFCore
{
    public class EFPositionEntryRepository : EFRepository<PositionEntry>, IPositionEntryRepository
    {
        public EFPositionEntryRepository(DbSet<PositionEntry> dbSet) : base(dbSet)
        {
        }

        public List<PositionEntry> FilterAccuracy(double accuracy)
        {
            return dbSet.Where(pe => pe.Accuracy <= 100).ToList();
        }

        public List<PositionEntry> TakeLast(int count)
        {
            return dbSet.OrderByDescending(x => x.Timestamp).Take(count).ToList();
        }
    }
}
