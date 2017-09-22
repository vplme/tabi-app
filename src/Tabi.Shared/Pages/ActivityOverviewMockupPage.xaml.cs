using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using MvvmHelpers;
using Tabi.Logging;
using Tabi.Model;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityOverviewMockupPage : ContentPage
    {
        ActivityOverviewViewModel ViewModel => vm ?? (vm = BindingContext as ActivityOverviewViewModel);
        ActivityOverviewViewModel vm;

        RouteTracker routeTracker = new RouteTracker();
        //StopResolver stopResolver = new StopResolver();

        public ActivityOverviewMockupPage()
        {
            InitializeComponent();

            BindingContext = new ActivityOverviewViewModel(this.Navigation);
            ViewModel.Title = "Overzicht";

            //List<Track> tracks1 = new List<Track>();
            //tracks1.Add(new Track() { Color = Color.Blue, Height = 8, Text = "7 min - Lopen" });
            //tracks1.Add(new Track() { Color = Color.LimeGreen, Height = 40, Text = "26 min - Bus" });
            //tracks1.Add(new Track() { Color = Color.Yellow, Height = 100, Text = "39 min - Trein " });
            //tracks1.Add(new Track() { Color = Color.Red, Height = 28, Text = "14 min - Metro " });
            //tracks1.Add(new Track() { Color = Color.Blue, Height = 10, Text = "4 min - Lopen " });

            //List<Track> tracks2 = new List<Track>();
            //tracks2.Add(new Track() { Color = Color.Blue, Height = 10, Text = "4 min - Lopen " });
            //tracks2.Add(new Track() { Color = Color.Red, Height = 28, Text = "14 min - Metro " });
            //tracks2.Add(new Track() { Color = Color.Yellow, Height = 100, Text = "39 min - Trein " });
            //tracks2.Add(new Track() { Color = Color.LimeGreen, Height = 40, Text = "26 min - Bus" });
            //tracks2.Add(new Track() { Color = Color.Blue, Height = 8, Text = "7 min - Lopen" });

            //ActivityEntry entry1 = new ActivityEntry()
            //{
            //    Name = "Thuis",
            //    Time = "00:00 - - 7:14",
            //};
            //entry1.Tracks.AddRange(tracks1);
            //ViewModel.ActivityEntries.Add(entry1);

            //ActivityEntry entry2 = new ActivityEntry()
            //{
            //    Name = "CBS",
            //    Time = "08:51 - 17:01",
            //};

            //entry2.Tracks.AddRange(tracks2);
            //ViewModel.ActivityEntries.Add(entry2);
        }

        void OnStopClicked(object sender, EventArgs args)
        {
            //StopDetailPage page = new StopDetailPage();

            Navigation.PushAsync(new ActivityDetailPage());
        }

        void OnPlaceClicked(object sender, EventArgs args)
        {
            Navigation.PushAsync(new ActivityDetailPage());
        }

    }
}
