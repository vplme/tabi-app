using System;
using System.Collections.Generic;
using Autofac;
using Tabi.Shared.ViewModels;
using Xamarin.Forms;

namespace Tabi.Shared.Pages.OnBoarding
{
    public partial class MotionAccessPage : ContentPage
    {
        MotionAccessViewModel ViewModel => vm ?? (vm = BindingContext as MotionAccessViewModel);
        MotionAccessViewModel vm;

        public MotionAccessPage()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<MotionAccessViewModel>();
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
