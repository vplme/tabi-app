using System;
using Microsoft.EntityFrameworkCore;
using Tabi.DataObjects;

namespace Tabi.DataStorage.EFCore
{
    public class EFUserRepository : EFRepository<User>, IUserRepository
    {
        public EFUserRepository(DbSet<User> dbSet) : base(dbSet)
        {
        }
    }
}
