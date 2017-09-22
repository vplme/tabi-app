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
            ViewModel.UpdateStopVisits();

            //if (first)
            //{
            //    UpdateStops();
            //    LoadData();
            //    first = false;
            //}






        }


        void RefreshClicked(object sender, EventArgs arg)
        {

            ViewModel.UpdateStopVisits();
        }

    }
}
