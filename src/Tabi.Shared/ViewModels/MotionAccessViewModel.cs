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
        public MotionAccessViewModel()
        {
            MotionAccessCommand = new Command(async () => await RequestMotionAccessAsync());

            ContinueCommand = new Command(async () => await CheckAndContinueAsync());
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

        private void PermissionStatusSetViewModel(PermissionStatus status)
        {
            switch (status)
            {
                case PermissionStatus.Granted:
                    MotionButtonStyle = (Style)Application.Current.Resources["successButtonStyle"];
                    break;
                case PermissionStatus.Denied:
                    MotionButtonStyle = (Style)Application.Current.Resources["warningButtonStyle"];
                    break;
            }
        }

        public async Task OnAppearingAsync()
        {
            await CheckMotionPermissionAsync();
        }

        public async Task CheckMotionPermissionAsync()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Sensors);
            PermissionStatusSetViewModel(status);
        }

        private async Task RequestMotionAccessAsync()
        {
            PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Sensors);

            // If permission was denied before, open App Settings.
            if (status == PermissionStatus.Denied && Device.RuntimePlatform == Device.iOS)
            {
                bool openSettings = await Page.DisplayAlert(AppResources.MotionTitleLabel, AppResources.MotionIntroLabel, AppResources.Settings, AppResources.CancelText);

                if (openSettings)
                {
                    CrossPermissions.Current.OpenAppSettings();
                }
            }
            else if (status != PermissionStatus.Granted)
            {
                var statusMap = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Sensors);
                if (statusMap.ContainsKey(Permission.Sensors))
                {
                    status = statusMap[Permission.Sensors];
                }
            }
            PermissionStatusSetViewModel(status);
        }

        private async Task Next()
        {
            Navigation.InsertPageBefore(new ThanksPage(), Page);
            await Navigation.PopAsync();
        }

        private async Task CheckAndContinueAsync()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Sensors);

                IDeviceInfo deviceInfo = Plugin.DeviceInfo.CrossDeviceInfo.Current;

                // Also allow simulator to pass
                if (status == PermissionStatus.Granted || status == PermissionStatus.Disabled)
                {
                    await Next();
                }
                else
                {
                    await Page.DisplayAlert(AppResources.MotionTitleLabel, AppResources.MotionIntroLabel, AppResources.OkText);
                }
            }
        }

    }
}
