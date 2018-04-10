using System;
using System.Collections.Generic;
using System.Linq;  
using System.Windows.Input;
using Plugin.DeviceInfo.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Tabi.Pages;
using Tabi.Shared.Controls;
using Tabi.Shared.IntroViews;
using Tabi.Shared.Resx;
using TabiApiClient;
using Xamarin.Forms;

namespace Tabi.Shared.ViewModels
{
    public class IntroViewModel : BaseViewModel
    {
        IntroPage introPage;

        public List<View> Views { get; set; } = new List<View>();
        private int nextView = 0;

        public ICommand NextCommand { get; set; }
        public ICommand PermissionCheckCommand { get; set; }
        public ICommand PermissionsCommand { get; set; }
        public ICommand LoginCommand { protected set; get; }
        public ICommand SensorPermissionCommand { get; set; }

        
        public View NextView
        {
            get
            {
                View view;
                if (nextView < Views.Count)
                {
                    view = Views[nextView];
                    nextView++;
                }
                else
                {
                    view = Views.Last();
                }
                return view;
            }
        }

        public bool isLoading;
        public bool IsLoading
        {
            get
            {
                return isLoading;
            }
            set
            {
                isLoading = value;
                OnPropertyChanged();
            }
        }

        public string username;
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
                OnPropertyChanged();
            }
        }

        public string password;
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }

        public Color permissionLocationButtonColor = (Color)Application.Current.Resources["blueButtonColor"];
        public Color PermissionLocationButtonColor
        {
            get
            {
                return permissionLocationButtonColor;
            }
            set
            {
                permissionLocationButtonColor = value;
                OnPropertyChanged();
            }
        }


        public Color permissionCheckButtonColor = Color.Gray;
        public Color PermissionCheckButtonColor
        {
            get
            {
                return permissionCheckButtonColor;
            }
            set
            {
                permissionCheckButtonColor = value;
                OnPropertyChanged();
            }
        }

        public Color permissionSensorButtonColor = (Color)Application.Current.Resources["blueButtonColor"];
        public Color PermissionSensorButtonColor
        {
            get
            {
                return permissionSensorButtonColor;
            }
            set
            {
                permissionSensorButtonColor = value;
                OnPropertyChanged();
            }
        }

        private bool permissionsGiven;
        public bool PermissionsGiven
        {
            get
            {
                return permissionsGiven;
            }
            set
            {
                permissionsGiven = value;
                OnPropertyChanged();
            }
        }

        private bool locationPermissionGiven;
        public bool LocationPermissionGiven
        {
            get
            {
                return locationPermissionGiven;
            }
            set
            {
                locationPermissionGiven = value;
                OnPropertyChanged();
            }
        }

        private bool sensorPermissionGiven;
        public bool SensorPermissionGiven
        {
            get
            {
                return sensorPermissionGiven;
            }
            set
            {
                sensorPermissionGiven = value;
                OnPropertyChanged();
            }
        }

        private bool isIOS;
        public bool IsIOS
        {
            get
            {
                return isIOS;
            }
            set
            {
                isIOS = value;
                OnPropertyChanged();
            }
        }

        private void GoNextView()
        {
            introPage.Content = this.NextView;
        }


        public IntroViewModel(IntroPage ip)
        {
            introPage = ip;

            Views.Add(new FirstIntroView());
            Views.Add(new LoginIntroView());
            Views.Add(new PermIntroView());

            IsIOS = Device.RuntimePlatform == Device.iOS;
            if (!IsIOS)
            {
                SensorPermissionGiven = true;
            }

            
            LoginCommand = new Command(async (obj) =>
            {
                ApiClient ac = new ApiClient(App.Configuration["api-url"]);
                TokenResult tokenResult = null;
                try
                {
                    IsLoading = true;
                    tokenResult = await ac.Authenticate(username, password);
                    IsLoading = false;

                }
                catch (Exception exc)
                {
                    await introPage.DisplayAlert("Error occured", $"Error: {exc}", "Ok");
                    return;
                }
                if (tokenResult != null)
                {
                    Settings.Current.Username = username;
                    Settings.Current.Password = password;

                    if (await ac.GetDevice(Settings.Current.Device) != null)
                    {
                        // Device is already registered
                    }
                    else
                    {
                        IDeviceInfo deviceInfo = Plugin.DeviceInfo.CrossDeviceInfo.Current;
                        bool success = await ac.RegisterDevice(Settings.Current.Device, deviceInfo.Model, deviceInfo.Version, deviceInfo.Manufacturer);
                        if (!success)
                        {
                            await introPage.DisplayAlert(AppResources.ErrorOccurredTitle, "Problem registering device", "OK");

                        }
                    }

                    GoNextView();
                }
                else
                {
                    await introPage.DisplayAlert(AppResources.LoginFailureTitle, AppResources.LoginFailureText, "OK");
                }
            });

            PermissionsCommand = new Command(async (obj) =>
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await introPage.DisplayAlert(
                            AppResources.LocationPermissionRationaleTitle,
                            AppResources.LocationPermissionRationaleText,
                            AppResources.OkText);
                    }
                    if (status == PermissionStatus.Denied && Device.RuntimePlatform == Device.iOS)
                    {
                        await introPage.DisplayAlert(
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
                    PermissionLocationButtonColor = (Color)Application.Current.Resources["greenButtonColor"];
                    LocationPermissionGiven = true;
                    CheckAllPermissionsGiven();
                }
            });

            PermissionCheckCommand = new Command((obj) =>
            {
                //triggered when?
                if (PermissionsGiven)
                {
                    introPage.Navigation.PopModalAsync();
                    Settings.Current.PermissionsGranted = true;
                    Settings.Current.Tracking = true;
                }
            });

            NextCommand = new Command((obj) => { GoNextView(); });

            SensorPermissionCommand = new Command(async (obj) => {
                //permission from iOS for usage of pedometer
                // double check if OS is iOS
                if (Device.RuntimePlatform == Device.iOS)
                {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Sensors);
                    if (status != PermissionStatus.Granted)
                    {
                        if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Sensors))
                        {
                            await introPage.DisplayAlert("Need sensor access", "Tabi requires sensor access", "OK");
                        }

                        var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Sensors);
                        //Best practice to always check that the key exists
                        if (results.ContainsKey(Permission.Location))
                            status = results[Permission.Location];
                    }

                    if (status == PermissionStatus.Granted)
                    {
                        SensorPermissionGiven = true;
                        CheckAllPermissionsGiven();
                    }
                    else if (status != PermissionStatus.Unknown)
                    {
                        await introPage.DisplayAlert("Sensor access denied", "Can not continue, try again.", "OK");
                    }
                }
                
            });
            GoNextView();
        }

        private void CheckAllPermissionsGiven()
        {
            if (LocationPermissionGiven && SensorPermissionGiven)
            {
                PermissionsGiven = true;
                PermissionCheckButtonColor = (Color)Application.Current.Resources["blueButtonColor"];
            }
        }
    }
}
