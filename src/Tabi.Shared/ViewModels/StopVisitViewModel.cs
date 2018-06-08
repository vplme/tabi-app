using System;
using MvvmHelpers;
using Tabi.DataObjects;

namespace Tabi.Shared.ViewModels
{
    public class StopVisitViewModel : BaseViewModel
    {
        private StopVisit _stopVisit;

        public StopVisitViewModel(StopVisit stopVisit)
        {
            _stopVisit = stopVisit ?? throw new ArgumentNullException(nameof(stopVisit));

            ResetViewModel();
        }

        public StopVisit StopVisit { get => _stopVisit; }

        private string name;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public double Latitude
        {
            get => _stopVisit.Stop.Latitude;
        }

        public double Longitude
        {
            get => _stopVisit.Stop.Longitude;
        }

        public double Accuracy
        {
            get => _stopVisit.StopAccuracy;
        }

        public void ResetViewModel()
        {
            Name = _stopVisit.Stop.Name ?? "";
        }

        public Stop SaveViewModelToStop()
        {
            Stop stop = _stopVisit.Stop;
            stop.Name = name;
            stop.Timestamp = DateTimeOffset.Now;

            return stop;
        }

    }
}
