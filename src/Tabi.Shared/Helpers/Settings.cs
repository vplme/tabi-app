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

        public string Device
        {
            get => AppSettings.GetValueOrDefault(nameof(Device), string.Empty);
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

        public long PositionLastUpload
        {
            // Cast default value as long otherwise it will choose the int typed GetValueOrDefault
            get => AppSettings.GetValueOrDefault(nameof(PositionLastUpload), (long)0);
            set
            {
                if (value == PositionLastUpload)
                    return;

                AppSettings.AddOrUpdateValue(nameof(PositionLastUpload), value);
                OnPropertyChanged();
            }
        }

        public long BatteryInfoLastUpload
        {
            // Cast default value as long otherwise it will choose the int typed GetValueOrDefault
            get => AppSettings.GetValueOrDefault(nameof(BatteryInfoLastUpload), (long)0);
            set
            {
                if (value == BatteryInfoLastUpload)
                    return;

                AppSettings.AddOrUpdateValue(nameof(BatteryInfoLastUpload), value);
                OnPropertyChanged();
            }
        }

        public long LogsLastUpload
        {
            // Cast default value as long otherwise it will choose the int typed GetValueOrDefault
            get => AppSettings.GetValueOrDefault(nameof(LogsLastUpload), (long)0);
            set
            {
                if (value == LogsLastUpload)
                    return;

                AppSettings.AddOrUpdateValue(nameof(LogsLastUpload), value);
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
