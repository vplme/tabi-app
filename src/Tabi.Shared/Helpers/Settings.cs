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

        public long TracksLastUpload
        {
            get => AppSettings.GetValueOrDefault(nameof(TracksLastUpload), (long)0);
            set
            {
                if (value == TracksLastUpload)
                {
                    return;
                }
                AppSettings.AddOrUpdateValue(nameof(TracksLastUpload), value);
                OnPropertyChanged();
            }
        }

        public long TransportModeLastUpload
        {
            get => AppSettings.GetValueOrDefault(nameof(TransportModeLastUpload), (long)0);
            set
            {
                if (value == TransportModeLastUpload)
                {
                    return;
                }
                AppSettings.AddOrUpdateValue(nameof(TransportModeLastUpload), value);
                OnPropertyChanged();
            }
        }

        public long AccelerometerLastUpload
        {
            get => AppSettings.GetValueOrDefault(nameof(AccelerometerLastUpload), (long)0);
            set
            {
                if (value == AccelerometerLastUpload)
                {
                    return;
                }
                AppSettings.AddOrUpdateValue(nameof(AccelerometerLastUpload), value);
                OnPropertyChanged();
            }
        }

        public long GyroscopeLastUpload
        {
            get => AppSettings.GetValueOrDefault(nameof(GyroscopeLastUpload), (long)0);
            set
            {
                if (value == GyroscopeLastUpload)
                {
                    return;
                }
                AppSettings.AddOrUpdateValue(nameof(GyroscopeLastUpload), value);
                OnPropertyChanged();
            }
        }

        public long MagnetometerLastUpload
        {
            get => AppSettings.GetValueOrDefault(nameof(MagnetometerLastUpload), (long)0);
            set
            {
                if (value == MagnetometerLastUpload)
                {
                    return;
                }
                AppSettings.AddOrUpdateValue(nameof(MagnetometerLastUpload), value);
                OnPropertyChanged();
            }
        }

        public long LinearAccelerationLastUpload
        {
            get => AppSettings.GetValueOrDefault(nameof(LinearAccelerationLastUpload), (long)0);
            set
            {
                if (value == LinearAccelerationLastUpload)
                {
                    return;
                }
                AppSettings.AddOrUpdateValue(nameof(LinearAccelerationLastUpload), value);
                OnPropertyChanged();
            }
        }

        public long GravityLastUpload
        {
            get => AppSettings.GetValueOrDefault(nameof(GravityLastUpload), (long)0);
            set
            {
                if (value == GravityLastUpload)
                {
                    return;
                }
                AppSettings.AddOrUpdateValue(nameof(GravityLastUpload), value);
                OnPropertyChanged();
            }
        }
        public long OrientationLastUpload
        {
            get => AppSettings.GetValueOrDefault(nameof(OrientationLastUpload), (long)0);
            set
            {
                if (value == OrientationLastUpload)
                {
                    return;
                }
                AppSettings.AddOrUpdateValue(nameof(OrientationLastUpload), value);
                OnPropertyChanged();
            }
        }

        public long QuaternionLastUpload
        {
            get => AppSettings.GetValueOrDefault(nameof(QuaternionLastUpload), (long)0);
            set
            {
                if (value == QuaternionLastUpload)
                {
                    return;
                }
                AppSettings.AddOrUpdateValue(nameof(QuaternionLastUpload), value);
                OnPropertyChanged();
            }
        }

        public long SensorMeasurementSessionLastUpload
        {
            get => AppSettings.GetValueOrDefault(nameof(SensorMeasurementSessionLastUpload), (long)0);
            set
            {
                if (value == SensorMeasurementSessionLastUpload)
                {
                    return;
                }
                AppSettings.AddOrUpdateValue(nameof(SensorMeasurementSessionLastUpload), value);
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
