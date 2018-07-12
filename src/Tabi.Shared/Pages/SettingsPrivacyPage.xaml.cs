using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Tabi.Shared
{
    public partial class SettingsPrivacyPage : ContentPage
    {
        SettingsViewModel ViewModel => vm ?? (vm = BindingContext as SettingsViewModel);
        SettingsViewModel vm;

        public SettingsPrivacyPage(SettingsViewModel viewModel)
        {
            BindingContext = viewModel;

            InitializeComponent();
        }
    }
}
