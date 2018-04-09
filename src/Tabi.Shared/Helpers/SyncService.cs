using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Tabi.DataObjects;
using Tabi.DataStorage;
using TabiApiClient;
using TabiApiClient.Messages;

namespace Tabi.iOS.Helpers
{
    public class SyncService
    {
        ApiClient ApiClient;
        DateTimeOffset lastAutoUpload;

        public SyncService(string url = "https://tabi.0x2a.site")
        {
            ApiClient = new ApiClient(url);
        }

        public async Task Login()
        {
            await ApiClient.Authenticate(Settings.Current.Username, Settings.Current.Password);

        }

        public async Task AutoUpload(TimeSpan window, bool wifiOnly = true)
        {
            if (DateTimeOffset.Now - window >= lastAutoUpload)
            {
                try
                {
                    await UploadAll(wifiOnly);
                    lastAutoUpload = DateTimeOffset.Now;
                }
                catch (Exception e)
                {
                    Log.Error($"UploadAll exception {e.Message}: {e.StackTrace}");
                }
            }
        }


        public async Task UploadAll(bool wifiOnly = true)
        {
            var wifi = Plugin.Connectivity.Abstractions.ConnectionType.WiFi;
            var connectionTypes = CrossConnectivity.Current.ConnectionTypes;

            if (!wifiOnly || connectionTypes.Contains(wifi))
            {
                Console.WriteLine("login");
                await Login();
                Console.WriteLine("upload positions");
                await UploadPositions();
                Console.WriteLine("upload logs");
                await UploadLogs();
                Console.WriteLine("upload battery info");
                await UploadBatteryInfo();
                Console.WriteLine("upload stop visits");
                await UploadStopVisits();

                // tracks && sensordata
                // select only tracks & sensordata from tracks which are complete
                TrackEntry lastTrack = App.RepoManager.TrackEntryRepository.LastCompletedTrackEntry();


                Console.WriteLine("upload tracks");
                var success = await UploadTracks();
                if (success)
                {
                    // remove old tracks
                    // var removeoldTracksSuccess = RemoveOldTracks();

                    Console.WriteLine("upload sensordata");

                    // UPLOADSUCCESS IS BOOL ARRAY IN ORDER OF TASKS
                    bool[] uploadSuccess = await Task.WhenAll(
                        UploadSensorMeasurementSessions(),
                        UploadAccelerometerData(),
                        UploadGyroscopeData(),
                        UploadMagnetometerData(),
                        UploadLinearAccelerationData(),
                        UploadGravityData(),
                        UploadOrientationData(),
                        UploadQuaternionData(),
                        UploadTransportationModes()
                        );


                    var removeOldDataSuccess = await RemoveOldSensorData(uploadSuccess);
                    Console.WriteLine("RemoveSuccess:");
                    foreach (var item in removeOldDataSuccess)
                    {
                        Console.WriteLine(item);
                    }
                }

                await ValidateCounts();
            }
        }
        

        public async Task UploadPositions()
        {
            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.PositionLastUpload, TimeSpan.Zero);
            List<PositionEntry> positions = App.RepoManager.PositionEntryRepository.After(lastUpload);
            if (positions.Count() > 0)
            {
                bool success = await ApiClient.PostPositions(Settings.Current.Device, positions);
                if (!success)
                {
                    Log.Error("Could not send positions");
                    return;
                }
                else
                {
                    Settings.Current.PositionLastUpload = positions.Last().Timestamp.Ticks;
                }
            }
        }

        private bool RemoveOldTracks()
        {
            // Tracks should be kept 5 days!!!
            int amountOfDaysAgo = 5;

            try
            {
                List<TrackEntry> tracksWithChildren = new List<TrackEntry>();
                // get tracks before range
                List<TrackEntry> tracks = App.RepoManager.TrackEntryRepository.GetRangeByEndTime(DateTimeOffset.MinValue, DateTimeOffset.Now.AddDays(amountOfDaysAgo)).ToList();
                foreach (var track in tracks)
                {
                    tracksWithChildren.Add(App.RepoManager.TrackEntryRepository.GetWithChildren(track.Id));
                }

                foreach (var trackWithChildren in tracksWithChildren)
                {
                    App.RepoManager.TrackEntryRepository.Remove(trackWithChildren);
                }


                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Log.Error(e.ToString());
                return false;
            }
        }

        private Task<bool[]> RemoveOldSensorData(bool[] uploadSuccess)
        {
            // creating list of items where uploads are succeeded to be removed as a batch
            List<Task<bool>> toBeRemoved = new List<Task<bool>>();
            if (uploadSuccess[0]) {
                toBeRemoved.Add(RemoveOldSensorMeasurementSessions());
            }
            if (uploadSuccess[1])
            {
                toBeRemoved.Add(RemoveOldAccelerometerData());
            }
            if (uploadSuccess[2])
            {
                toBeRemoved.Add(RemoveOldGyroscopeData());
            }
            if (uploadSuccess[3])
            {
                toBeRemoved.Add(RemoveOldMagnetometerData());
            }
            if (uploadSuccess[4])
            {
                toBeRemoved.Add(RemoveOldLinearAccelerationData());
            }
            if (uploadSuccess[5])
            {
                toBeRemoved.Add(RemoveOldGravityData());
            }
            if (uploadSuccess[6])
            {
                toBeRemoved.Add(RemoveOldOrientationData());
            }
            if (uploadSuccess[7])
            {
                toBeRemoved.Add(RemoveOldQuaternionData());
            }
            
            var removeOldSensorDataSuccess = Task.WhenAll(
                        toBeRemoved
                    );
            return removeOldSensorDataSuccess;
        }

        private async Task<bool> RemoveOldSensorMeasurementSessions()
        {
            ISensorMeasurementSessionRepository sensorMeasurementRepository = App.RepoManager.SensorMeasurementSessionRepository;
            return await Task.Run(() => sensorMeasurementRepository.RemoveRangeBeforeTimestamp(new DateTimeOffset(Settings.Current.SensorMeasurementSessionLastUpload, TimeSpan.Zero)));
        }

        private async Task<bool> RemoveOldAccelerometerData()
        {
            ISensorRepository<Accelerometer> accelerometerRepository = App.RepoManager.AccelerometerRepository;
            Console.WriteLine("amount of accelerometerdata before: " + accelerometerRepository.Count());
            var removedSuccessfully = await Task.Run(() => accelerometerRepository.RemoveRangeBeforeTimestamp(new DateTimeOffset(Settings.Current.AccelerometerLastUpload, TimeSpan.Zero)));
            if (removedSuccessfully)
            {
                Console.WriteLine("amount of accelerometerdata after: " + accelerometerRepository.Count());
            }

            return removedSuccessfully;
        }

        private async Task<bool> RemoveOldGyroscopeData()
        {
            ISensorRepository<Gyroscope> gyroscopeRepository = App.RepoManager.GyroscopeRepository;

            return await Task.Run(() => gyroscopeRepository.RemoveRangeBeforeTimestamp(new DateTimeOffset(Settings.Current.GyroscopeLastUpload, TimeSpan.Zero)));
        }

        private async Task<bool> RemoveOldMagnetometerData()
        {
            ISensorRepository<Magnetometer> magnetometerRepository = App.RepoManager.MagnetometerRepository;

            return await Task.Run(() => magnetometerRepository.RemoveRangeBeforeTimestamp(new DateTimeOffset(Settings.Current.MagnetometerLastUpload, TimeSpan.Zero)));
        }

        private async Task<bool> RemoveOldLinearAccelerationData()
        {
            ISensorRepository<LinearAcceleration> linearAccelerationRepository = App.RepoManager.LinearAccelerationRepository;

            return await Task.Run(() => linearAccelerationRepository.RemoveRangeBeforeTimestamp(new DateTimeOffset(Settings.Current.LinearAccelerationLastUpload, TimeSpan.Zero)));
        }

        private async Task<bool> RemoveOldGravityData()
        {
            ISensorRepository<Gravity> gravityRepository = App.RepoManager.GravityRepository;

            return await Task.Run(() => gravityRepository.RemoveRangeBeforeTimestamp(new DateTimeOffset(Settings.Current.GravityLastUpload, TimeSpan.Zero)));
        }

        private async Task<bool> RemoveOldOrientationData()
        {
            ISensorRepository<Orientation> orientationRepository = App.RepoManager.OrientationRepository;

            return await Task.Run(() => orientationRepository.RemoveRangeBeforeTimestamp(new DateTimeOffset(Settings.Current.OrientationLastUpload, TimeSpan.Zero)));
        }

        private async Task<bool> RemoveOldQuaternionData()
        {
            ISensorRepository<Quaternion> quaternionRepository = App.RepoManager.QuaternionRepository;

            return await Task.Run(() => quaternionRepository.RemoveRangeBeforeTimestamp(new DateTimeOffset(Settings.Current.QuaternionLastUpload, TimeSpan.Zero)));
        }


        public async Task<bool> ValidateCounts()
        {
            bool valid = true;

            DateTimeOffset lastPositionUploaded = new DateTimeOffset(Settings.Current.PositionLastUpload, TimeSpan.Zero);
            DateTimeOffset lastLogUploaded = new DateTimeOffset(Settings.Current.LogsLastUpload, TimeSpan.Zero);
            DateTimeOffset lastBatteryInfoUploaded = new DateTimeOffset(Settings.Current.BatteryInfoLastUpload, TimeSpan.Zero);

            int positionCount = App.RepoManager.PositionEntryRepository.CountBefore(lastPositionUploaded);
            int logCount = App.RepoManager.LogEntryRepository.CountBefore(lastLogUploaded);
            int batteryCount = App.RepoManager.BatteryEntryRepository.CountBefore(lastBatteryInfoUploaded);

            DeviceCounts counts = await ApiClient.GetDeviceCounts(Settings.Current.Device);
            if (counts != null)
            {
                if (counts.Positions != positionCount)
                {
                    Log.Error($"Position count invalid local {positionCount} remote {counts.Positions}");
                    valid = false;
                }
                if (counts.Logs != logCount)
                {
                    Log.Error($"Log count invalid local {logCount} remote {counts.Logs}");
                    valid = false;
                }
                if (counts.BatteryInfos != batteryCount)
                {
                    Log.Error($"Battery count invalid local {batteryCount} remote {counts.BatteryInfos}");
                    valid = false;
                }
            }
            else
            {
                Log.Error("Could not validate counts with remote server");
            }

            return valid;
        }


        public async Task<bool> UploadLogs()
        {
            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.LogsLastUpload, TimeSpan.Zero);

            List<LogEntry> logs = App.RepoManager.LogEntryRepository.After(lastUpload);
            if (logs.Count() > 0)
            {
                bool success = await ApiClient.PostLogs(Settings.Current.Device, logs);
                if (!success)
                {
                    Log.Error("Could not send logs");
                    return false;
                }
                else
                {
                    Settings.Current.LogsLastUpload = logs.Last().Timestamp.Ticks;
                    //App.RepoManager.LogEntryRepository.ClearLogsBefore(logs.Last().Timestamp);
                }
            }
            return true;
        }


        public async Task UploadStopVisits()
        {
            List<StopVisit> stopVisits = App.RepoManager.StopVisitRepository.GetAll().ToList();
            // Retrieve stops
            foreach (StopVisit sv in stopVisits)
            {
                sv.Stop = App.RepoManager.StopRepository.Get(sv.StopId);
            }


            bool success = await ApiClient.PostStopVisits(Settings.Current.Device, stopVisits);
            if (!success)
            {
                Log.Error("Could not send stopvisits");
                return;
            }
        }

        public async Task<bool> UploadBatteryInfo()
        {
            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.BatteryInfoLastUpload, TimeSpan.Zero);
            List<BatteryEntry> batteryEntries = App.RepoManager.BatteryEntryRepository.After(lastUpload);
            if (batteryEntries.Any())
            {
                bool success = await ApiClient.PostBatteryData(Settings.Current.Device, batteryEntries);
                if (!success)
                {
                    Log.Error($"Tried to send {batteryEntries.Count()} batterydata but failed");
                    return false;
                }
                else
                {
                    Settings.Current.BatteryInfoLastUpload = batteryEntries.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                }
            }

            return true;
        }

        public async Task<bool> UploadAccelerometerData()
        {
            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.AccelerometerLastUpload, TimeSpan.Zero);
            try
            {
                List<Accelerometer> accelerometerData = App.RepoManager.AccelerometerRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

                bool success = await ApiClient.PostAccelerometerData(Settings.Current.Device, accelerometerData);

                if (!success)
                {
                    Log.Error($"Tried to send {accelerometerData.Count} accelerometerdata but failed");
                    return false;
                }

                Settings.Current.AccelerometerLastUpload = accelerometerData.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload accelerometerdata " + e);
                return false;
            }
        }

        public async Task<bool> UploadGyroscopeData()
        {
            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.GyroscopeLastUpload, TimeSpan.Zero);
            try
            {
                //get gyroscopedata
                List<Gyroscope> gyroscopeData = App.RepoManager.GyroscopeRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

                bool success = await ApiClient.PostGyroscopeData(Settings.Current.Device, gyroscopeData);

                if (!success)
                {
                    Log.Error($"Tried to send {gyroscopeData.Count} gyroscopedata but failed");
                    return false;
                }

                Settings.Current.GyroscopeLastUpload = gyroscopeData.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload gyroscopedata " + e);
                return false;
            }
        }

        public async Task<bool> UploadMagnetometerData()
        {
            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.MagnetometerLastUpload, TimeSpan.Zero);
            try
            {
                //get magnetometerdata
                List<Magnetometer> magnetometerData = App.RepoManager.MagnetometerRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

                bool success = await ApiClient.PostMagnetometerData(Settings.Current.Device, magnetometerData);

                if (!success)
                {
                    Log.Error($"Tried to send {magnetometerData.Count} magnetometerdata but failed");
                    return false;
                }

                Settings.Current.MagnetometerLastUpload = magnetometerData.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload magnetometerdata " + e);
                return false;
            }
        }
        private async Task<bool> UploadQuaternionData()
        {
            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.QuaternionLastUpload, TimeSpan.Zero);
            try
            {
                List<Quaternion> quaternionData = App.RepoManager.QuaternionRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

                bool success = await ApiClient.PostQuaternionData(Settings.Current.Device, quaternionData);

                if (!success)
                {
                    Log.Error($"Tried to send {quaternionData.Count} quaterniondata but failed");
                    return false;
                }

                Settings.Current.QuaternionLastUpload = quaternionData.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload quaterniondata " + e);
                return false;
            }
        }

        private async Task<bool> UploadOrientationData()
        {
            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.OrientationLastUpload, TimeSpan.Zero);
            try
            {
                List<Orientation> orientationData = App.RepoManager.OrientationRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

                bool success = await ApiClient.PostOrientationData(Settings.Current.Device, orientationData);

                if (!success)
                {
                    Log.Error($"Tried to send {orientationData.Count} orientationdata but failed");
                    return false;
                }

                Settings.Current.OrientationLastUpload = orientationData.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload orientation " + e);
                return false;
            }
        }

        private async Task<bool> UploadGravityData()
        {
            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.GravityLastUpload , TimeSpan.Zero);
            try
            {
                List<Gravity> gravityData = App.RepoManager.GravityRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

                bool success = await ApiClient.PostGravityData(Settings.Current.Device, gravityData);

                if (!success)
                {
                    Log.Error($"Tried to send {gravityData.Count} gravitydata but failed");
                    return false;
                }

                Settings.Current.GravityLastUpload = gravityData.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload gravitydata " + e);
                return false;
            }
        }

        private async Task<bool> UploadLinearAccelerationData()
        {
            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.LinearAccelerationLastUpload, TimeSpan.Zero);
            try
            {
                List<LinearAcceleration> linearAccelerationData = App.RepoManager.LinearAccelerationRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

                bool success = await ApiClient.PostLinearAccelerationData(Settings.Current.Device, linearAccelerationData);

                if (!success)
                {
                    Log.Error($"Tried to send {linearAccelerationData.Count} linearaccelerationdata but failed");
                    return false;
                }

                Settings.Current.LinearAccelerationLastUpload = linearAccelerationData.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload linearaccelerationdata " + e);
                return false;
            }
        }

        public async Task<bool> UploadSensorMeasurementSessions()
        {
            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.SensorMeasurementSessionLastUpload, TimeSpan.Zero);
            try
            {
                //get sensormeasurementsessiondata
                List<SensorMeasurementSession> sensorMeasurementSessions = App.RepoManager.SensorMeasurementSessionRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

                bool success = await ApiClient.PostSensorMeasurementSessions(Settings.Current.Device, sensorMeasurementSessions);

                if (!success)
                {
                    Log.Error($"Tried to send {sensorMeasurementSessions.Count} sensormeasurementsessions but failed");
                    return false;
                }
                Settings.Current.SensorMeasurementSessionLastUpload = sensorMeasurementSessions.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload sensormeasurementsessions " + e);
                return false;
            }
        }

        public async Task<bool> UploadTransportationModes()
        {
            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.TransportModeLastUpload, TimeSpan.Zero);

            try
            {
                List<TabiApiClient.Models.TransportationMode> transportModes = new List<TabiApiClient.Models.TransportationMode>();

                //get transportmodes that are between lastuploaded and lasttrackentry
                List<TransportationModeEntry> transportationModeEntries = App.RepoManager.TransportationModeRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

                foreach (var transportationMode in transportationModeEntries)
                {
                    TabiApiClient.Models.TransportationMode transportationModeDTO = new TabiApiClient.Models.TransportationMode()
                    {
                        TrackId = transportationMode.TrackId,
                        Timestamp = transportationMode.Timestamp,

                        Walk = transportationMode.Walk,
                        Run = transportationMode.Run,
                        MobilityScooter = transportationMode.MobilityScooter,
                        Car = transportationMode.Car,
                        Bike = transportationMode.Bike,
                        Moped = transportationMode.Moped,
                        Scooter = transportationMode.Scooter,
                        Motorcycle = transportationMode.Motorcycle,
                        Train = transportationMode.Train,
                        Subway = transportationMode.Subway,
                        Tram = transportationMode.Tram,
                        Bus = transportationMode.Bus,
                        Other = transportationMode.Other
                    };
                    transportModes.Add(transportationModeDTO);
                }

                bool success = await ApiClient.PostTransportationModes(Settings.Current.Device, transportModes);

                if (!success)
                {
                    Log.Error($"Tried to send {transportModes.Count} transportationModes but failed");
                    return false;
                }

                Settings.Current.TransportModeLastUpload = transportationModeEntries.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;


                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload transportationModes " + e);
                return false;
            }
        }

        public async Task<bool> UploadTracks()
        {
            //TODO upload tracks and keep track of last one uploaded
            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.TracksLastUpload, TimeSpan.Zero);

            try
            {
                //gets tracks that are completed and between lastuploadtime and LastCompletedTrackEntry
                List<TrackEntry> trackEntries = App.RepoManager.TrackEntryRepository.GetRangeByEndTime(lastUpload, DateTimeOffset.MaxValue).ToList();
                trackEntries.Remove(trackEntries.Last());


                // convert to trackDTO
                List<TabiApiClient.Models.TrackEntry> trackDTO = new List<TabiApiClient.Models.TrackEntry>();
                foreach (var trackEntryWithChildren in trackEntries)
                {
                    trackDTO.Add(new TabiApiClient.Models.TrackEntry()
                    {
                        Id = trackEntryWithChildren.Id,
                        StartTime = trackEntryWithChildren.StartTime,
                        EndTime = trackEntryWithChildren.EndTime
                    });
                }

                bool success = await ApiClient.PostTrackEntries(Settings.Current.Device, trackDTO);

                if (!success)
                {
                    Log.Error($"Tried to send {trackEntries.Count} trackEntries but failed");
                    return false;
                }

                Settings.Current.TracksLastUpload = trackEntries.Last().EndTime.Ticks;

                return true;
            } catch (Exception e)
            {
                Log.Error("Could not upload tracks " + e);
                return false;
            }

        }

        
    }
}
