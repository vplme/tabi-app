using System;
using System.Diagnostics;
using Plugin.Permissions.Abstractions;
using Tabi.Extensions;
using Tabi.Model;
using Tabi.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabiPage : ContentPage
    {
        public TabiPage()
        {
            InitializeComponent();
            Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
        }

        void MapButton_Clicked(object sender, System.EventArgs e)
        {
            DayMapPage page = new DayMapPage();
            Navigation.PushAsync(page);
        }

        void SettingsButton_Clicked(object sender, System.EventArgs e)
        {
            SettingsPage sPage = new SettingsPage();
            Navigation.PushAsync(sPage);
        }

        void ActivityOverviewButton_Clicked(object sender, System.EventArgs e)
        {
            ActivityOverviewPage sPage = new ActivityOverviewPage();
            Navigation.PushAsync(sPage);
        }

        void ActivityOverviewMockupButton_Clicked(object sender, System.EventArgs e)
        {
            ActivityOverviewMockupPage sPage = new ActivityOverviewMockupPage();
            Navigation.PushAsync(sPage);
        }
    }
}
