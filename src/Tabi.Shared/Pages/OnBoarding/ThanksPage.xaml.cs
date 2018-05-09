using System;
using System.Collections.Generic;
using Autofac;
using Tabi.Shared.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi.Shared.Pages.OnBoarding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ThanksPage : ContentPage
    {
        ThanksViewModel ViewModel => vm ?? (vm = BindingContext as ThanksViewModel);
        ThanksViewModel vm;

        public ThanksPage()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<ThanksViewModel>();
            ViewModel.Navigation = this.Navigation;
            ViewModel.Page = this;
        }
    }
}
