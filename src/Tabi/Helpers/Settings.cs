using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Tabi.ViewModels;

namespace Tabi.Helpers
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

        public bool Tracking
        {
            get => AppSettings.GetValueOrDefault(nameof(Tracking), false);
            set
            {
                if (value == Tracking)
                    return;

                AppSettings.AddOrUpdateValue(nameof(Tracking), value);
                OnPropertyChanged();
            }
        }

        public bool SensorMeasurements
        {
            get => AppSettings.GetValueOrDefault(nameof(SensorMeasurements), false);
            set
            {
                if (value == SensorMeasurements)
                    return;

                AppSettings.AddOrUpdateValue(nameof(SensorMeasurements), value);
                OnPropertyChanged();
            }
        }

        public bool PermissionsGranted
        {
            get => AppSettings.GetValueOrDefault(nameof(PermissionsGranted), false);
            set
            {
                if (value == PermissionsGranted)
                    return;

                AppSettings.AddOrUpdateValue(nameof(PermissionsGranted), value);
                OnPropertyChanged();
            }
        }

        public bool AnalyticsGranted
        {
            get => AppSettings.GetValueOrDefault(nameof(AnalyticsGranted), true);
            set
            {
                if (value == AnalyticsGranted)
                    return;

                AppSettings.AddOrUpdateValue(nameof(AnalyticsGranted), value);
                OnPropertyChanged();
            }
        }

        public bool CrashesGranted
        {
            get => AppSettings.GetValueOrDefault(nameof(CrashesGranted), true);
            set
            {
                if (value == CrashesGranted)
                    return;

                AppSettings.AddOrUpdateValue(nameof(CrashesGranted), value);
                OnPropertyChanged();
            }
        }

        public bool AutoUpload
        {
            get => AppSettings.GetValueOrDefault(nameof(AutoUpload), true);
            set
            {
                if (value == AutoUpload)
                    return;

                AppSettings.AddOrUpdateValue(nameof(AutoUpload), value);
                OnPropertyChanged();
            }
        }

        public bool WifiOnly
        {
            get => AppSettings.GetValueOrDefault(nameof(WifiOnly), true);
            set
            {
                if (value == WifiOnly)
                    return;

                AppSettings.AddOrUpdateValue(nameof(WifiOnly), value);
                OnPropertyChanged();
            }
        }

        public bool ShowTour
        {
            get => AppSettings.GetValueOrDefault(nameof(ShowTour), false);
            set
            {
                if (value == ShowTour)
                    return;

                AppSettings.AddOrUpdateValue(nameof(ShowTour), value);
                OnPropertyChanged();
            }
        }

        public bool Developer
        {
            get => AppSettings.GetValueOrDefault(nameof(Developer), false);
            set
            {
                if (value == Developer)
                    return;

                AppSettings.AddOrUpdateValue(nameof(Developer), value);
                OnPropertyChanged();
            }
        }

        public int Device
        {
            get => AppSettings.GetValueOrDefault(nameof(Device), 0);
            set
            {
                if (value == Device)
                    return;

                AppSettings.AddOrUpdateValue(nameof(Device), value);
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => AppSettings.GetValueOrDefault(nameof(Username), string.Empty);
            set
            {
                if (value == Username)
                    return;

                AppSettings.AddOrUpdateValue(nameof(Username), value);
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => AppSettings.GetValueOrDefault(nameof(Password), string.Empty);
            set
            {
                if (value == Password)
                    return;

                AppSettings.AddOrUpdateValue(nameof(Password), value);
                OnPropertyChanged();
            }
        }

        public string DeviceToken
        {
            get => AppSettings.GetValueOrDefault(nameof(DeviceToken), string.Empty);
            set
            {
                if (value == DeviceToken)
                    return;

                AppSettings.AddOrUpdateValue(nameof(DeviceToken), value);
                OnPropertyChanged();
            }
        }

        public long LastUpload
        {
            get => AppSettings.GetValueOrDefault(nameof(LastUpload), (long)0);
            set
            {
                if (value == LastUpload)
                {
                    return;
                }
                AppSettings.AddOrUpdateValue(nameof(LastUpload), value);
                OnPropertyChanged();
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
