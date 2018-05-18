using System;
using MvvmHelpers;
using Newtonsoft.Json;
using SQLite;
namespace Tabi.DataObjects
{
    public class Stop : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}
