using System;
using MvvmHelpers;
using SQLite;
namespace Tabi.DataObjects
{
    public class Stop : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private double latitude;
        public double Latitude
        {
            get => latitude;
            set => SetProperty(ref latitude, value);
        }

        double longitude;
        public double Longitude
        {
            get => longitude;
            set => SetProperty(ref longitude, value);
        }
    }
}
