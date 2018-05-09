using System;
using System.Collections.Generic;
using Autofac;
using Tabi.Shared.ViewModels;
using Xamarin.Forms;

namespace Tabi.Shared.Pages.OnBoarding
{
    public partial class LocationAccessPage : ContentPage
    {
        LocationAccessViewModel ViewModel => vm ?? (vm = BindingContext as LocationAccessViewModel);
        LocationAccessViewModel vm;

        public LocationAccessPage()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<LocationAccessViewModel>();
            ViewModel.Page = this;
            ViewModel.Navigation = Navigation;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel.OnAppearingAsync();
        }
    }
}
