using System;
using System.Diagnostics;
using Xamarin.Forms;
using PCLStorage;
using System.ComponentModel;
using Tabi.Helpers;
using Autofac;
using Xamarin.Forms.Xaml;

namespace Tabi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        SettingsViewModel ViewModel => vm ?? (vm = BindingContext as SettingsViewModel);
        SettingsViewModel vm;

        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<SettingsViewModel>(new TypedParameter(typeof(INavigation), Navigation));
        }
    }
}
