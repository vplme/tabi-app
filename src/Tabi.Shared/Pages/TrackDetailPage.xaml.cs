using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Tabi.Pages
{
    public partial class TrackDetailPage : ContentPage
    {
        TrackDetailViewModel ViewModel => vm ?? (vm = BindingContext as TrackDetailViewModel);
        TrackDetailViewModel vm;

        Track track;

        public TrackDetailPage(Track track)
        {
            InitializeComponent();

            this.track = track ?? throw new ArgumentNullException(nameof(track));

            BindingContext = App.Container.Resolve<TrackDetailViewModel>();
            ViewModel.Navigation = Navigation;

            routeMap.HeightRequest = App.ScreenHeight * 0.30;

            ViewModel.TrackEntry = track.TrackEntry;

            ShowRoute();
        }

        void ShowRoute()
        {
            routeMap.Lines.Add(ViewModel.GetMapLine());

            routeMap.MoveToRegion(ViewModel.AveragePosition());

            routeMap.DrawRoute();
        }
    }
}
