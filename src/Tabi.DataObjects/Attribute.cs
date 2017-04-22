using System;
using SQLite;

namespace Tabi.DataObjects
{
    public class Attribute
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public Guid DeviceId { get; set; }

        [Ignore]
        public Device Device { get; set; }

        public string Tag { get; set; }

        public string Value { get; set; }
    }
}
