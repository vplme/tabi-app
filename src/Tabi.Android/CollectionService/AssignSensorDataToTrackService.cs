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
            // service for resolve data per 5 minutes
            Timer timer = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);
            timer.AutoReset = true;
            timer.Elapsed += TimerElapsed;
            timer.Start();

            return StartCommandResult.Sticky;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {

            _trackEntryRepository.Add(new TrackEntry() { StartTime = DateTimeOffset.Now.AddHours(-1), EndTime = DateTimeOffset.Now });
            var testTack = _trackEntryRepository.GetAll().Last();
            testTack.UniqueKey = Guid.NewGuid();

            bool accelerometerUpdated = _accelerometerRepository.UpdateTrackKey(testTack, "Accelerometer");
            bool gyroscopeUpdated = _gyroscopeRepository.UpdateTrackKey(testTack, "Gyroscope");
            bool magnetometerUpdated = _magnetometerRepository.UpdateTrackKey(testTack, "Magnetometer");
            bool linearAcceleration = _linearAccelerationRepository.UpdateTrackKey(testTack, "LinearAcceleration");
            bool gravityUpdated = _linearAccelerationRepository.UpdateTrackKey(testTack, "Gravity");
            bool orientationUpdated = _orientationRepository.UpdateTrackKey(testTack, "Orientation");
            bool quaternionUpdated = _quaternionRepository.UpdateTrackKey(testTack, "Quaternion");
            bool sensorMeasurementSessionUpdated = _sensorMeasurementSessionRepository.UpdateTrackKey(testTack);

            var accelerometer = _accelerometerRepository.GetAll().Where(x => x.TrackEntryKey == testTack.UniqueKey).ToList();
            Console.WriteLine(accelerometer.Count); 

            //resolve the data
            DataResolver dataResolver = new DataResolver();
            dataResolver.ResolveData(DateTimeOffset.MinValue, DateTimeOffset.Now);

            //get all information we need
            List<TrackEntry> trackEntries = _trackEntryRepository.GetAll().ToList();

            Console.WriteLine("Trackentries: " + trackEntries.Count);

            //check if there are any tracks
            if (!trackEntries.Any())
            {
                return;
            }
            

            //if it is the first track made
            if (lastTrack.Id == 0)
            {
                lastTrack = trackEntries.Last();
            }

            //if there is a difference between last recorded track and latest track maded
            if (lastTrack != trackEntries.Last())
            {
                //resolve last recorded track
                //TrackEntry toBeResolvedTrack = trackEntries.First(t => t.Id == lastTrack.Id);
                //bool accelerometerUpdated = _accelerometerRepository.UpdateTrackKey(toBeResolvedTrack, "Accelerometer");
                //bool gyroscopeUpdated = _gyroscopeRepository.UpdateTrackKey(toBeResolvedTrack, "Gyroscope");
                //bool magnetometerUpdated = _magnetometerRepository.UpdateTrackKey(toBeResolvedTrack, "Magnetometer");
                //bool linearAcceleration = _linearAccelerationRepository.UpdateTrackKey(toBeResolvedTrack, "LinearAcceleration");
                //bool gravityUpdated = _linearAccelerationRepository.UpdateTrackKey(toBeResolvedTrack, "Gravity");
                //bool orientationUpdated = _orientationRepository.UpdateTrackKey(toBeResolvedTrack, "Orientation");
                //bool quaternionUpdated = _quaternionRepository.UpdateTrackKey(toBeResolvedTrack, "Quaternion");
                //bool sensorMeasurementSessionUpdated = _sensorMeasurementSessionRepository.UpdateTrackKey(toBeResolvedTrack);


                lastTrack = trackEntries.Last();
                
            }
            
        }
    }
}