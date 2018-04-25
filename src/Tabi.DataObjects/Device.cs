using System;
using SQLite;

namespace Tabi.DataObjects
{
    public class Device
    {
        [PrimaryKey]
        public int Id { get; set; }

        public Guid UserId { get; set; }

        [Ignore]
        public User User { get; set; }

        public string Manufacturer { get; set; }

        public string Model { get; set; }

        public string OperatingSystem { get; set; }
    }
}
