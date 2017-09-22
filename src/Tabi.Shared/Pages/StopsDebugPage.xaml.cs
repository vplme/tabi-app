using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tabi.DataObjects;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Tabi.Pages;
using Tabi.Logging;

namespace Tabi
{
    public partial class StopsDebugPage : ContentPage
    {
        ObservableCollection<Stop> stops;
        //StopResolver resolve;
//        Repository<Stop> stopRepo;

        public StopsDebugPage()
        {
            InitializeComponent();

            stops = new ObservableCollection<Stop>();
            StopsView.ItemsSource = stops;

            //resolve = new StopResolver();
        }

        async void ResolveVisits()
        {
            List<int> stopIds = new List<int>();
            //var listVisits = await resolve.GetStopsBetweenAsync(DateTimeOffset.MinValue, DateTimeOffset.Now);
            //foreach (StopVisit sv in listVisits)
            //{
            //    if (!stopIds.Contains(sv.StopId))
            //    {
            //        Stop s = await stopRepo.Get(sv.StopId);
            //        stops.Add(s);
            //        stopIds.Add(sv.StopId);
            //    }
            //}
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            stops.Clear();
            ResolveVisits();
        }

        void OnSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //if (e.SelectedItem != null)
            //{
            //    Debug.WriteLine("StopsDebugPage OnSelected");
            //    ListView view = (ListView)sender;
            //    Stop stop = (Stop)e.SelectedItem;

            //    view.SelectedItem = null;

            //    //Stop stop = e.SelectedItem as Stop;
            //    StopDetailPage page = new StopDetailPage(stop);
            //    Navigation.PushAsync(page);
            //}
        }
    }
}
