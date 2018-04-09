using System;
using System.Collections.Generic;
using Tabi.ViewModels;
using Xamarin.Forms;

namespace Tabi.Pages
{
    public partial class TrackDetailPage : ContentPage
    {
        TrackDetailViewModel ViewModel => vm ?? (vm = BindingContext as TrackDetailViewModel);
        TrackDetailViewModel vm;

        public TrackDetailPage(Track track)
        {
            InitializeComponent();
            BindingContext = new TrackDetailViewModel(this.Navigation);
            routeMap.HeightRequest = App.ScreenHeight * 0.30;

            ViewModel.TrackEntry = track.TrackEntry;
        }
    }
}
