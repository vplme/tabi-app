﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AppCenter.Analytics;
using Tabi.Model;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityOverviewPage : ContentPage
    {
        ActivityOverviewViewModel ViewModel => vm ?? (vm = BindingContext as ActivityOverviewViewModel);
        ActivityOverviewViewModel vm;

        Dictionary<DateTimeOffset, DateTimeOffset> lastLoads = new Dictionary<DateTimeOffset, DateTimeOffset>();

        public ActivityOverviewPage()
        {
            InitializeComponent();

            BindingContext = App.Container.Resolve<ActivityOverviewViewModel>(new TypedParameter(typeof(INavigation), Navigation));
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }


        protected async override void OnAppearing()
        {
            await ViewModel.OnAppearing();
            await UpdateAsync();
        }

        async Task UpdateAsync()
        {
            ViewModel.IsBusy = true;
            Task uiTask = Task.Delay(2000);
            await ViewModel.UpdateStopVisitsAsync();
            // Show the UI for at least a second..
            await uiTask;
            ViewModel.IsBusy = false;
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
                        Analytics.TrackEvent("Stop clicked");
                        page = new StopDetailPage(ae.StopVisit);
                    }
                    else if (ae.ShowTrack)
                    {
                        Analytics.TrackEvent("Track clicked");
                        page = new TrackDetailPage(ae.Track);
                    }

                    await Navigation.PushAsync(page);
                }

                ((ListView)sender).SelectedItem = null;

            }
        }
    }
}