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
            MotionAccessCommand = new Command(async () => RequestMotionAccess());

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

        private void RequestMotionAccess()
        {
            var status = _extraPermission.CheckMotionPermission();
            if (status == PermissionAuthorization.Authorized)
            {
                MotionButtonStyle = (Style)Application.Current.Resources["successButtonStyle"];
            }
            else
            {
                _extraPermission.RequestMotionPermission();
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
                if (status == PermissionAuthorization.Authorized || !_extraPermission.IsMotionAvailable())
                {
                    await Next();
                }
                else
                {
                    await Page.DisplayAlert("Motion", AppResources.MotionIntroLabel, "Ok");
                }

            }
        }

    }
}
