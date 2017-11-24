using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Tabi.Pages;
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

        public bool permissionsGiven;
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

            LoginCommand = new Command(async (obj) =>
            {
                ApiClient ac = new ApiClient();
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
                        bool success = await ac.RegisterDevice(Settings.Current.Device);
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
                    PermissionsGiven = true;
                    PermissionCheckButtonColor = (Color)Application.Current.Resources["blueButtonColor"];
                }
            });

            PermissionCheckCommand = new Command((obj) =>
            {
                if (PermissionsGiven)
                {
                    introPage.Navigation.PopModalAsync();
                    Settings.Current.PermissionsGranted = true;
                    Settings.Current.Tracking = true;
                }
            });

            NextCommand = new Command((obj) => { GoNextView(); });
            GoNextView();
        }
    }
}
