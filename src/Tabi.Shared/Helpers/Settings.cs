using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Tabi
{
    public class Settings : BaseViewModel
    {
        private static Settings instance;

        public static Settings Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new Settings();
                }
                return instance;
            }
        }
        public Settings() { }

        private const string trackingKey = "tracking_key";
        private readonly bool trackingDefault = false;

        public bool Tracking
        {
            get { return AppSettings.GetValueOrDefault<bool>(trackingKey, trackingDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue<bool>(trackingKey, value))
                {
                    OnPropertyChanged();
                }
            }
        }

		private const string permissionsGrantedKey = "permissions_granted_key";
		private readonly bool permissionsGrantedDefault = false;

		public bool PermissionsGranted
		{
			get { return AppSettings.GetValueOrDefault<bool>(permissionsGrantedKey, permissionsGrantedDefault); }
			set
			{
				if (AppSettings.AddOrUpdateValue<bool>(permissionsGrantedKey, value))
				{
					OnPropertyChanged();
				}
			}
		}


        private const string developerKey = "developer_key";
        private readonly bool developerDefault = false;

        public bool Developer
        {
            get { return AppSettings.GetValueOrDefault<bool>(developerKey, developerDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue<bool>(developerKey, value))
                {
                    OnPropertyChanged();
                }
            }
        }

        private const string deviceKey = "device_guid";
        private readonly string deviceDefault = "";

        public string Device
        {
            get { return AppSettings.GetValueOrDefault<string>(deviceKey, deviceDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue<string>(deviceKey, value))
                {
                    OnPropertyChanged();
                }
            }
        }

        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }
    }
}
