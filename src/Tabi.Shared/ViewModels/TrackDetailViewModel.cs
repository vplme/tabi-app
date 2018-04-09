using System;
using System.Collections.Generic;
using System.Windows.Input;
using MvvmHelpers;
using Tabi.DataObjects;
using Tabi.Pages;
using Tabi.Shared.Model;
using Xamarin.Forms;

namespace Tabi.ViewModels
{
    public class TrackDetailViewModel : ObservableObject
    {
        public ICommand TransportModeSelectionCommand { get; protected set; }

        private INavigation navigation;

        public TrackEntry TrackEntry { get; set; }

        public TrackDetailViewModel(INavigation navigation)
        {
            this.navigation = navigation;

            TransportModeSelectionCommand = new Command(async () =>
            {
                await navigation.PushAsync(new TransportSelectionPage(TrackEntry));
            });
        }
    }
}
