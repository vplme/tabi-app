using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Tabi.Shared.Pages
{
    public partial class AboutSettings : ContentPage
    {
        SettingsViewModel ViewModel => vm ?? (vm = BindingContext as SettingsViewModel);
        SettingsViewModel vm;

        public AboutSettings(SettingsViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
        }
    }
}
