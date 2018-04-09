using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Tabi.Core;
using Tabi.DataObjects;
using Tabi.DataStorage;
using static Android.OS.PowerManager;

namespace Tabi.Droid.CollectionService
{
    [Service]
    public class AssignSensorDataToTrackService : Service
    {
        private readonly ITrackEntryRepository _trackEntryRepository;
        private readonly ISensorRepository<Accelerometer> _accelerometerRepository;
        private readonly ISensorRepository<Gyroscope> _gyroscopeRepository;
        private readonly ISensorRepository<Magnetometer> _magnetometerRepository;
        private readonly ISensorRepository<LinearAcceleration> _linearAccelerationRepository;
        private readonly ISensorRepository<Orientation> _orientationRepository;
        private readonly ISensorRepository<Quaternion> _quaternionRepository;
        private readonly ISensorRepository<Gravity> _gravityRepository;
        private readonly ISensorMeasurementSessionRepository _sensorMeasurementSessionRepository;

        private AssignSensorDataToTrackServiceBinder _binder;
        private TrackEntry lastTrack;

        public AssignSensorDataToTrackService()
        {
            _trackEntryRepository = App.RepoManager.TrackEntryRepository;
            _accelerometerRepository = App.RepoManager.AccelerometerRepository;
            _gyroscopeRepository = App.RepoManager.GyroscopeRepository;
            _magnetometerRepository = App.RepoManager.MagnetometerRepository;
            _linearAccelerationRepository = App.RepoManager.LinearAccelerationRepository;
            _orientationRepository = App.RepoManager.OrientationRepository;
            _quaternionRepository = App.RepoManager.QuaternionRepository;
            _gravityRepository = App.RepoManager.GravityRepository;
            _sensorMeasurementSessionRepository = App.RepoManager.SensorMeasurementSessionRepository;

            lastTrack = new TrackEntry();
        }

        public override IBinder OnBind(Intent intent)
        {
            _binder = new AssignSensorDataToTrackServiceBinder(this);
            return _binder;
        }

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            PowerManager sv = (Android.OS.PowerManager)GetSystemService(PowerService);
            WakeLock wklock = sv.NewWakeLock(WakeLockFlags.Partial, "TABI_assign_sensor_to_track");
            wklock.Acquire();

            // service for resolve data per 30 minutes
            Timer timer = new Timer(TimeSpan.FromMinutes(30).TotalMilliseconds);
            timer.AutoReset = true;
            timer.Elapsed += TimerElapsed;
            timer.Start();
            
            return StartCommandResult.Sticky;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {

            //Task.Run(() =>
            //{
            //    //resolve the data
            //    DataResolver dataResolver = new DataResolver();
            //    dataResolver.ResolveData(DateTimeOffset.MinValue, DateTimeOffset.Now);

            //    //get all information we need
            //    List<TrackEntry> trackEntries = _trackEntryRepository.GetAll().OrderBy(x => x.StartTime).ToList();

            //    Console.WriteLine("Trackentries: " + trackEntries.Count);

            //    //check if there are any tracks
            //    if (!trackEntries.Any())
            //    {
            //        return;
            //    }

            //    //if it is the first track made
            //    if (lastTrack.Id == Guid.Empty)
            //    {
            //        lastTrack = trackEntries.Last();
            //    }

            //    //if there is a new track maded.. resolve the lastTrack and replace with newest track
            //    if (lastTrack.Id != trackEntries.Last().Id)
            //    {                    
            //        Console.WriteLine("begin resolving" + DateTime.Now);

            //        //resolve last recorded track
            //        bool accelerometerUpdated = _accelerometerRepository.UpdateTrackKey(lastTrack, "Accelerometer");
            //        bool gyroscopeUpdated = _gyroscopeRepository.UpdateTrackKey(lastTrack, "Gyroscope");
            //        bool magnetometerUpdated = _magnetometerRepository.UpdateTrackKey(lastTrack, "Magnetometer");
            //        bool linearAcceleration = _linearAccelerationRepository.UpdateTrackKey(lastTrack, "LinearAcceleration");
            //        bool gravityUpdated = _linearAccelerationRepository.UpdateTrackKey(lastTrack, "Gravity");
            //        bool orientationUpdated = _orientationRepository.UpdateTrackKey(lastTrack, "Orientation");
            //        bool quaternionUpdated = _quaternionRepository.UpdateTrackKey(lastTrack, "Quaternion");
            //        bool sensorMeasurementSessionUpdated = _sensorMeasurementSessionRepository.UpdateTrackKey(lastTrack);

            //        Console.WriteLine("Done resolving" + DateTime.Now);

            //        this.lastTrack = trackEntries.Last();
            //    }
            //});
            
        }
    }
}