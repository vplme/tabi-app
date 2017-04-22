using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Tabi
{
    public partial class StopDetailPage : ContentPage
    {
        StopDetailViewModel ViewModel => vm ?? (vm = BindingContext as StopDetailViewModel);
        StopDetailViewModel vm;


        IStopRepository stopRepository = App.RepoManager.StopRepository;

        public StopDetailPage(Stop s)
        {
            InitializeComponent();
            routeMap.HeightRequest = App.ScreenHeight * 0.30;
            SetupPage(s);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

        }

        public string StopName
        {
            get
            {
                return ViewModel.Stop.Name;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            SaveStop();
        }

        private void SetupPage(Stop s)
        {
            //Retrieve latest version from db
            Stop stop = stopRepository.Get(s.Id);
            Debug.WriteLine("After:" + stop);

            SetMapLocation(stop);
            BindingContext = new StopDetailViewModel(stop);
        }



        private void SetMapLocation(Stop s)
        {
            if (s == null)
            {
                return;
            }
            var pos = new Xamarin.Forms.Maps.Position(s.Latitude, s.Longitude);

            string labelPin = "Stop";
            if (s.Name != null)
            {
                labelPin = s.Name;
            }

            routeMap.Pins.Add(new Xamarin.Forms.Maps.Pin() { Label = labelPin, Position = pos });
            routeMap.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromMeters(300)));
        }

        private void SaveStop()
        {
            stopRepository.Add(ViewModel.Stop);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            vm = null;
        }
    }
}
