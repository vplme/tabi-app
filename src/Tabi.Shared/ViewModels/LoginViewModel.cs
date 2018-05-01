﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Plugin.DeviceInfo.Abstractions;
using Tabi.Shared.Pages.OnBoarding;
using Tabi.Shared.Resx;
using TabiApiClient;
using TabiApiClient.Messages;
using Xamarin.Forms;

namespace Tabi.Shared.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;

        public LoginViewModel(ApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

            LoginCommand = new Command(async () => await Login());

        }

        public INavigation Navigation { get; set; }

        public Page Page { get; set; }

        private bool isLoggingIn;
        public bool IsLoggingIn
        {
            get
            {
                return isLoggingIn;
            }
            set
            {
                SetProperty<bool>(ref isLoggingIn, value);
            }
        }

        private string username;
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                SetProperty<string>(ref username, value);
            }
        }

        private string password;
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                SetProperty<string>(ref password, value);
            }
        }

        public ICommand LoginCommand { get; set; }


        private async Task Next()
        {
            Page page = Navigation.NavigationStack.First();
            if (page is WelcomePage)
            {
                Navigation.RemovePage(page);
            }

            Navigation.InsertPageBefore(new LocationAccessPage(), Page);
            await Navigation.PopAsync();
        }

        public async Task Login()
        {
            IsLoggingIn = true;

            TokenResult tokenResult = null;
            try
            {
                tokenResult = await _apiClient.Authenticate(Username, Password);
            }
            catch (Exception e)
            {
                Log.Error(e);
                await Page.DisplayAlert("Error occured", $"Error: {e}", "Ok");
            }

            if (tokenResult != null)
            {
                Settings.Current.Username = username;
                Settings.Current.Password = password;
                try
                {
                    if (await _apiClient.GetDevice(Settings.Current.Device) != null)
                    {
                        Log.Debug("already register");
                        // Device is already registered
                        await Next();

                    }
                    else
                    {
                        IDeviceInfo deviceInfo = Plugin.DeviceInfo.CrossDeviceInfo.Current;
                        DeviceMessage response = await _apiClient.RegisterDevice(deviceInfo.Model, deviceInfo.Version, deviceInfo.Manufacturer);
                        if (response.ID == 0)
                        {
                            await Page.DisplayAlert(AppResources.ErrorOccurredTitle, "Problem registering device", "OK");
                        }
                        else
                        {
                            Settings.Current.Device = response.ID;
                            await Next();
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    await Page.DisplayAlert("Error occured", $"Error: {e}", "Ok");
                }

            }
            else
            {
                await Page.DisplayAlert(AppResources.LoginFailureTitle, AppResources.LoginFailureText, "OK");
            }

            IsLoggingIn = false;

        }
    }
}
