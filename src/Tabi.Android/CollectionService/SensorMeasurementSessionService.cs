using System;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Tabi.DataObjects;
using Tabi.DataStorage;

namespace Tabi.Droid.CollectionService
{
    [Service]
    public class SensorMeasurementSessionService : Service, ISensorEventListener
    {
        private readonly ISensorMeasurementSessionRepository _sensorMeasurementSessionRepository;
        private SensorMeasurementSessionServiceBinder _binder;
        private SensorMeasurementSession _sensorMeasurementSession;
        
        public SensorMeasurementSessionService()
        {
            _sensorMeasurementSessionRepository = App.RepoManager.SensorMeasurementSessionRepository;
            _sensorMeasurementSession = new SensorMeasurementSession();
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            
        }

        public override IBinder OnBind(Intent intent)
        {
            _binder = new SensorMeasurementSessionServiceBinder(this);
            return _binder;
        }

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Task.Run(() =>
            {
                //register sensors
                var sensorManager = (SensorManager)Application.Context.GetSystemService(Context.SensorService);


                Sensor ambientLight = sensorManager.GetDefaultSensor(SensorType.Light);
                sensorManager.RegisterListener(this, ambientLight, SensorDelay.Normal);


                //float degree = Math.round(event.values[0]);
                //tvHeading.setText("Heading: " + Float.toString(degree) + " degrees");
                Sensor compass = sensorManager.GetDefaultSensor(SensorType.Orientation);
                sensorManager.RegisterListener(this, compass, SensorDelay.Normal);

                Sensor pedometer = sensorManager.GetDefaultSensor(SensorType.StepCounter);
                sensorManager.RegisterListener(this, pedometer, SensorDelay.Normal);

                Sensor proximity = sensorManager.GetDefaultSensor(SensorType.Proximity);
                sensorManager.RegisterListener(this, proximity, SensorDelay.Normal);


                // service for measurements once per minute
                Timer timer = new Timer(60000);
                timer.AutoReset = true;
                timer.Elapsed += TimerElapsed;
                timer.Start();

            });

            return StartCommandResult.Sticky;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            // get data from _sensorMeasurementSession
            // insert datetimeoffset
            _sensorMeasurementSession.Timestamp = DateTimeOffset.Now;


            //battery info
            using (IntentFilter filter = new IntentFilter(Intent.ActionBatteryChanged))
            {
                using (Intent battery = Application.Context.RegisterReceiver(null, filter))
                {
                    int level = battery.GetIntExtra(BatteryManager.ExtraLevel, -1);
                    int scale = battery.GetIntExtra(BatteryManager.ExtraScale, -1);
                    int level_0_to_100 = Convert.ToInt32(Math.Floor(level * 100D / scale));
                    _sensorMeasurementSession.BatteryLevel = level_0_to_100;

                    // batterystatus
                    int status = battery.GetIntExtra(BatteryManager.ExtraStatus, -1);
                    switch (status)
                    {
                        case (int)BatteryStatus.Charging:
                            _sensorMeasurementSession.BatteryStatus = BatteryEntryState.Charging;
                            break;
                        case (int)BatteryStatus.Discharging:
                            _sensorMeasurementSession.BatteryStatus = BatteryEntryState.Discharging;
                            break;
                        case (int)BatteryStatus.NotCharging:
                            _sensorMeasurementSession.BatteryStatus = BatteryEntryState.NotCharging;
                            break;
                        case (int)BatteryStatus.Full:
                            _sensorMeasurementSession.BatteryStatus = BatteryEntryState.Full;
                            break;
                        case (int)BatteryStatus.Unknown:
                        _sensorMeasurementSession.BatteryStatus = BatteryEntryState.Unknown;
                        break;
                    }
                }
            }

            // push to database
            _sensorMeasurementSessionRepository.Add(_sensorMeasurementSession);

            //clean object
            _sensorMeasurementSession = new SensorMeasurementSession();
        }

        public void OnSensorChanged(SensorEvent e)
        {
            switch (e.Sensor.Type)
            {
                case SensorType.Light:
                    _sensorMeasurementSession.AmbientLight = Convert.ToInt32(e.Values[0]); // light level in lx
                    break;
                case SensorType.Orientation:
                    _sensorMeasurementSession.Compass = Convert.ToInt32(e.Values[0]); //azimuth magnetic north in degrees
                    break;
                case SensorType.Proximity:
                    _sensorMeasurementSession.Proximity = e.Values[0] < 5;  //returns bit more than 5.. check if smaller than 5 -> true
                    break;
                case SensorType.StepCounter:
                    _sensorMeasurementSession.Pedometer = Convert.ToInt32(e.Values[0]); // steps taken since reboot, resets on reboot
                    break;
                default:
                    break;
            }
        }
    }
}