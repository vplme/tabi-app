using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MvvmHelpers;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Logging;
using Tabi.Pages;
using Tabi.Shared.Resx;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityOverviewPage : ContentPage
    {
        ActivityOverviewViewModel ViewModel => vm ?? (vm = BindingContext as ActivityOverviewViewModel);
        ActivityOverviewViewModel vm;
        DateTimeOffset lastLoad;

        public ActivityOverviewPage()
        {
            InitializeComponent();

            BindingContext = new ActivityOverviewViewModel(this.Navigation);
            ViewModel.Title = AppResources.ActivityOverviewPageTitle;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (lastLoad < DateTimeOffset.Now.AddMinutes(-10))
            {
                Update();
            }
        }

        void Update()
        {
            ViewModel.UpdateStopVisits();
            lastLoad = DateTimeOffset.Now;
        }

        async void ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var lastPageType = Navigation.NavigationStack.Last().GetType();
                if (lastPageType != typeof(StopDetailPage) && lastPageType != typeof(TrackDetailPage))
                {
                    ActivityEntry ae = (ActivityEntry)e.SelectedItem;

                    Page page = null;
                    if (ae.ShowStop)
                    {
                        page = new StopDetailPage(ae.StopVisit);
                    }
                    else if (ae.ShowTrack)
                    {
                        page = new TrackDetailPage();
                    }

                    await Navigation.PushAsync(page);
                }

                ((ListView)sender).SelectedItem = null;

            }
        }
    }
}
