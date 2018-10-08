using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AppCenter.Analytics;
using Tabi.Controls;
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

            ViewModel.ActivityEntries.CollectionChanged += ActivityEntries_CollectionChanged;
        }

        void ActivityEntries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ActivityTrackLayout.Children.Clear();

            foreach(ActivityEntry ae in ViewModel.ActivityEntries)
            {
                if(ae.ShowStop)
                {
                    var t = new ActivityStopView
                    {
                        BindingContext = ae
                    };
                    ActivityTrackLayout.Children.Add(t);
                }
                else if(ae.ShowTrack)
                {
                    var t = new ActivityTrackView
                    {
                        BindingContext = ae
                    };
                    ActivityTrackLayout.Children.Add(t);
                }

            }
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
    }
}
