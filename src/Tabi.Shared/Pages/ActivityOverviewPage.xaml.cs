using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using MvvmHelpers;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Logging;
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


        void RefreshClicked(object sender, EventArgs arg)
        {
            Update();

            ViewModel.UpdateStopVisits();
        }

        void DaySelectorClicked(object sender, EventArgs arg)
        {
            
        }

        void ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                ActivityEntry ae = (ActivityEntry)e.SelectedItem;

                Page page = null;
                if (ae.ShowStop)
                {
                    page = new StopDetailPage(ae.StopVisit);
                }
                else if (ae.ShowTrack)
                {
                    page = new StopDetailPage(ae.StopVisit);
                }

                Navigation.PushAsync(page);

                ((ListView)sender).SelectedItem = null;

            }
        }
    }
}
