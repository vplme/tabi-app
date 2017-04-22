using System;
using Microsoft.EntityFrameworkCore;
using Tabi.DataObjects;

namespace Tabi.DataStorage.EFCore
{
    public class TabiContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<PositionEntry> PositionEntries { get; set; }
        public DbSet<MotionEntry> MotionEntries { get; set; }

        string path;

        public TabiContext() : base()
        {
            
        }


        public TabiContext(string path){
            this.path = path;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={path}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PositionEntry>().HasKey(p => new {p.DeviceId, p.Id});
            modelBuilder.Entity<MotionEntry>().HasKey(m => new { m.DeviceId, m.Id });

        }
    }
}
