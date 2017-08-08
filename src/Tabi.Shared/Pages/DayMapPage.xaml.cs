﻿using System;
using System.Collections.Generic;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Logging;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Tabi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DayMapPage : ContentPage
    {
        public DayMapPage()
        {
            InitializeComponent();

            //routeMap.ClearMap();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ShowPositions();
            ShowMap();
        }

         void ShowPositions()
        {
            IPositionEntryRepository positionEntryRepo = App.RepoManager.PositionEntryRepository;

            List<PositionEntry> positions = positionEntryRepo.FilterAccuracy(100);
            if (positions.Count == 0)
            {
                return;
            }
            PositionEntry avg = Util.AveragePosition(positions);
            routeMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(avg.Latitude, avg.Longitude), Distance.FromMiles(20.0)));

            // Remove previous route
            routeMap.RouteCoordinates.Clear();

            foreach (PositionEntry p in positions)
            {
                var xP = new Xamarin.Forms.Maps.Position(p.Latitude, p.Longitude);
                routeMap.RouteCoordinates.Add(xP);
            }
            routeMap.DrawRoute();
        }

        void DaySwitcher(object sender, EventArgs e)
        {

        }

        async void ShowMap()
        {
            routeMap.Pins.Clear();

            StopResolver resolver = new StopResolver();
            IStopRepository stopRepository = App.RepoManager.StopRepository;

            List<StopVisit> visits = await resolver.GetStopsBetweenAsync(DateTimeOffset.MinValue, DateTimeOffset.Now);

            foreach (StopVisit p in visits)
            {
                Stop st = stopRepository.Get(p.StopId);
                string labelPin = "Stop";

                if (st.Name != null)
                {
                    labelPin = st.Name;
                }

                var xP = new Xamarin.Forms.Maps.Position(st.Latitude, st.Longitude);
                Pin pin = new Xamarin.Forms.Maps.Pin() { Label = labelPin, Position = xP };
                pin.Clicked += (sender, e) =>
                {
                    StopDetailPage page = new StopDetailPage(st);
                    Navigation.PushAsync(page);
                };
                routeMap.Pins.Add(pin);
            }
        }
    }
}
