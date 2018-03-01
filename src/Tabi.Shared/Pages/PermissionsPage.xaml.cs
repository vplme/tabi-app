using System;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PermissionsPage : ContentPage
    {
        readonly IPermissions permissionsPlugin = CrossPermissions.Current;

        async void Grant_Clicked(object sender, EventArgs e)
        {
            var status = await permissionsPlugin.CheckPermissionStatusAsync(Permission.Location);

            // On iOS direct user to Settings if the user has denied access.
            if (status == PermissionStatus.Denied && Device.RuntimePlatform == Device.iOS)
            {
                var openSettingsAnswer = await DisplayAlert("Locatiebepaling vereist", "Zet alstublieft locatiebepaling aan.", "Open instellingen", "Annuleer");
                if (openSettingsAnswer)
                {
                    permissionsPlugin.OpenAppSettings();
                }
            }
            // 
            else if (status == PermissionStatus.Granted)
            {
                Settings.Current.PermissionsGranted = true;
                await Navigation.PopModalAsync();
            }
            else 
            {
                // iOS implementation of plugin returns a dictionary with the updated permissions (after the user chooses
                // grants or denies permission.
                // Android implementation does not return a dictionary with the latest permissions. Only
                // returns a dictionary with old permissions (or current if the requested permission is already granted)
                GrantButton.Text = "Next";
                var resultDict = await permissionsPlugin.RequestPermissionsAsync(Permission.Location);
                // Android: code after this point is not called.
                        
                PermissionStatus resultStatus = resultDict[Permission.Location];

                if (resultStatus == PermissionStatus.Granted)
                {
                    Settings.Current.PermissionsGranted = true;
                    await Navigation.PopModalAsync();
                }
            }
        }
        public PermissionsPage()
        {
            InitializeComponent();
        }

    }
}
