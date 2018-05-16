using System;
using MvvmHelpers;
using Tabi.DataObjects;

namespace Tabi.Shared.ViewModels
{
    public class StopViewModel : ObservableObject
    {
        private Stop _stop;

        public StopViewModel(Stop stop)
        {
            _stop = stop ?? throw new ArgumentNullException(nameof(stop));

            name = _stop.Name ?? "";
        }

        private string name;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string motive;

        public string Motive
        {
            get => motive;
            set => SetProperty(ref motive, value);
        }

        public double Latitude {
            get => _stop.Latitude;
        }

        public double Longitude
        {
            get => _stop.Longitude;
        }
    }
}
