using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Tabi.Controls;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Helpers;
using Tabi.Model;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Tabi.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrackDetailPage : ContentPage
    {
        TrackDetailViewModel ViewModel => vm ?? (vm = BindingContext as TrackDetailViewModel);
        TrackDetailViewModel vm;

        Track track;

        public TrackDetailPage(Track track)
        {
            this.track = track ?? throw new ArgumentNullException(nameof(track));

            InitializeComponent();

            BindingContext = App.Container.Resolve<TrackDetailViewModel>(
                new TypedParameter(typeof(INavigation), Navigation),
                new TypedParameter(typeof(TrackEntry), track.TrackEntry));

            routeMap.HeightRequest = App.ScreenHeight * 0.30;

            ShowRoute();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var adjust = Xamarin.Forms.Device.RuntimePlatform != Xamarin.Forms.Device.Android ? 1 : -ViewModel.DataItems.Count + 1;
            ListViewTrack.HeightRequest = (ViewModel.DataItems.Count * ListViewTrack.RowHeight) - adjust;
        }

        void ShowRoute()
        {
            routeMap.ClearMap();

            (Line line, Pin start, Pin end) = ViewModel.GetMapData();

            routeMap.Lines.Add(line);
            if (start != null && end != null)
            {
                routeMap.Pins.Add(start);
                routeMap.Pins.Add(end);
            }

            routeMap.MoveToRegion(ViewModel.AveragePosition());

            routeMap.DrawRoute();
        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }

            // Only run on Android. iOS uses TextCell with Command property.
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                ICommandable item = e.SelectedItem as ICommandable;
                if (item != null && item.Command != null && item.Command.CanExecute(null))
                {
                    item.Command.Execute(null);
                }
            }

            ((ListView)sender).SelectedItem = null;
        }
    }
}
