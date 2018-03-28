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
            //begin test code
            Console.WriteLine("create test track");

            //create transportationmode for test purposes
            List<TransportationMode> transportationModes = new List<TransportationMode>()
            {
                new TransportationMode() {Mode = TransportationModes.Bike },
                new TransportationMode() {Mode = TransportationModes.Car }
            };

            //add transportationmodes to database
            foreach (var transportationMode in transportationModes)
            {
                App.RepoManager.TransportationModeRepository.Add(transportationMode); 
            }
            
            //create track
            var myTrack = new TrackEntry()
            {
                StartTime = DateTimeOffset.Now.AddHours(-1),
                EndTime = DateTimeOffset.Now.AddHours(3),
                NextStopId = 5
            };

            //insert track in database
            _trackEntryRepository.Add(myTrack);

            //add transportationmodes to track
            myTrack.TransportationModes = transportationModes;

            //update track to databse
            var updateSuccess = _trackEntryRepository.UpdateWithChildren(myTrack);

            
            //get track with the transportationmodes
            var testTrack = _trackEntryRepository.GetWithChildren(myTrack.Id);
            foreach (var transportationMode in testTrack.TransportationModes)
            {
                Console.WriteLine(transportationMode);
            }


            //Console.WriteLine("start updating!: " + DateTime.Now);
            //bool accelerometerUpdated = _accelerometerRepository.UpdateTrackKey(testTrack, "Accelerometer");
            //bool gyroscopeUpdated = _gyroscopeRepository.UpdateTrackKey(testTrack, "Gyroscope");
            //bool magnetometerUpdated = _magnetometerRepository.UpdateTrackKey(testTrack, "Magnetometer");
            //bool linearAcceleration = _linearAccelerationRepository.UpdateTrackKey(testTrack, "LinearAcceleration");
            //bool gravityUpdated = _linearAccelerationRepository.UpdateTrackKey(testTrack, "Gravity");
            //bool orientationUpdated = _orientationRepository.UpdateTrackKey(testTrack, "Orientation");
            //bool quaternionUpdated = _quaternionRepository.UpdateTrackKey(testTrack, "Quaternion");
            //bool sensorMeasurementSessionUpdated = _sensorMeasurementSessionRepository.UpdateTrackKey(testTrack);
            //Console.WriteLine("finished updating: " + DateTime.Now);

            //end test code

            Task.Run(() =>
            {
                //resolve the data
                DataResolver dataResolver = new DataResolver();
                dataResolver.ResolveData(DateTimeOffset.MinValue, DateTimeOffset.Now);

                //get all information we need
                List<TrackEntry> trackEntries = _trackEntryRepository.GetAll().OrderBy(x => x.StartTime).ToList();

                Console.WriteLine("Trackentries: " + trackEntries.Count);

                //check if there are any tracks
                if (!trackEntries.Any())
                {
                    return;
                }

                //if it is the first track made
                if (lastTrack.Id == Guid.Empty)
                {
                    lastTrack = trackEntries.Last();
                }

                //if there is a new track maded.. resolve the lastTrack and replace with newest track
                if (lastTrack.Id != trackEntries.Last().Id)
                {                    
                    Console.WriteLine("begin resolving" + DateTime.Now);

                    //resolve last recorded track
                    //bool accelerometerUpdated = _accelerometerRepository.UpdateTrackKey(lastTrack, "Accelerometer");
                    //bool gyroscopeUpdated = _gyroscopeRepository.UpdateTrackKey(lastTrack, "Gyroscope");
                    //bool magnetometerUpdated = _magnetometerRepository.UpdateTrackKey(lastTrack, "Magnetometer");
                    //bool linearAcceleration = _linearAccelerationRepository.UpdateTrackKey(lastTrack, "LinearAcceleration");
                    //bool gravityUpdated = _linearAccelerationRepository.UpdateTrackKey(lastTrack, "Gravity");
                    //bool orientationUpdated = _orientationRepository.UpdateTrackKey(lastTrack, "Orientation");
                    //bool quaternionUpdated = _quaternionRepository.UpdateTrackKey(lastTrack, "Quaternion");
                    //bool sensorMeasurementSessionUpdated = _sensorMeasurementSessionRepository.UpdateTrackKey(lastTrack);

                    Console.WriteLine("Done resolving" + DateTime.Now);

                    this.lastTrack = trackEntries.Last();
                }
            });
            
        }
    }
}