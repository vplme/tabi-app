using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Plugin.DeviceInfo.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Tabi.Shared.Pages.OnBoarding;
using Tabi.Shared.Resx;
using Xamarin.Forms;

namespace Tabi.Shared.ViewModels
{
    public class MotionAccessViewModel : BaseViewModel
    {
        private readonly IExtraPermission _extraPermission;

        public MotionAccessViewModel(IExtraPermission extraPermission)
        {
            _extraPermission = extraPermission ?? throw new ArgumentNullException();
            MotionAccessCommand = new Command(async () => await RequestMotionAccess());

            ContinueCommand = new Command(async () => await CheckAndContinue());
        }



        public INavigation Navigation { get; set; }

        public Page Page { get; set; }


        public ICommand MotionAccessCommand { get; set; }

        public ICommand ContinueCommand { get; set; }


        private Style _motionButtonStyle = (Style)Application.Current.Resources["defaultButtonStyle"];
        public Style MotionButtonStyle
        {
            get
            {
                return _motionButtonStyle;
            }
            set
            {
                SetProperty<Style>(ref _motionButtonStyle, value);
            }
        }

        private async Task RequestMotionAccess()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                {
                    await Page.DisplayAlert(
                        AppResources.LocationPermissionRationaleTitle,
                        AppResources.LocationPermissionRationaleText,
                        AppResources.OkText);
                }
                if (status == PermissionStatus.Denied && Device.RuntimePlatform == Device.iOS)
                {
                    await Page.DisplayAlert(
                        AppResources.LocationPermissionDeniedOpenSettingsiOSTitle,
                        AppResources.LocationPermissionDeniedOpenSettingsiOSText,
                        AppResources.OkText);

                    CrossPermissions.Current.OpenAppSettings();
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                //Best practice to always check that the key exists
                if (results.ContainsKey(Permission.Location))
                    status = results[Permission.Location];
            }

            if (status == PermissionStatus.Granted)
            {
                MotionButtonStyle = (Style)Application.Current.Resources["successButtonStyle"];
            }
        }

        private async Task Next()
        {
            Navigation.InsertPageBefore(new ThanksPage(), Page);
            await Navigation.PopAsync();
        }

        private async Task CheckAndContinue()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                var status = _extraPermission.CheckMotionPermission();

                IDeviceInfo deviceInfo = Plugin.DeviceInfo.CrossDeviceInfo.Current;

                // Also allow simulator to pass
                if (status == PermissionAuthorization.Authorized || !deviceInfo.IsDevice)
                {
                    await Next();
                }
                else
                {
                    Page.DisplayAlert("Motion", "Enable motion access", "Understood");
                }

            }
        }

    }
}
