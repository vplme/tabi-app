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

            if (!ViewModel.Settings.Developer)
            {
                DeveloperSection.IsVisible = false;
            }

            VersionLabel.Text = DependencyService.Get<IVersion>().GetVersion();
            System.Diagnostics.Debug.WriteLine($"{two.TextColor}");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.Settings.PropertyChanged += Settings_PropertyChanged;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModel.Settings.PropertyChanged -= Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Debug.WriteLine("Settings_PropertyChanged" + e.PropertyName + " " + ViewModel.Settings.Developer);
            if (e.PropertyName == "Developer")
            {
                if (ViewModel.Settings.Developer && !DeveloperSection.IsVisible)
                {
                    DeveloperSection.IsVisible = true;
                }
                else if (!ViewModel.Settings.Developer && DeveloperSection.IsVisible)
                {
                    DeveloperSection.IsVisible = false;
                }
            }
        }

        async void SqliteOpen(object sender, EventArgs e)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            var t = await rootFolder.GetFileAsync("tabi");
            DependencyService.Get<IShareFile>().ShareFile(t.Path);
        }
    }
}
