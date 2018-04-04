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
                var success = await UploadTracks(lastTrack.EndTime);
                if (success)
                {
                    // remove old tracks
                    var removeoldTracksSuccess = RemoveOldTracks(lastTrack.EndTime);

                    Console.WriteLine("upload sensordata");

                    // UPLOADSUCCESS IS BOOL ARRAY IN ORDER OF TASKS
                    bool[] uploadSuccess = await Task.WhenAll(
                        UploadSensorMeasurementSessions(lastTrack.EndTime),
                        UploadAccelerometerData(lastTrack.EndTime),
                        UploadGyroscopeData(lastTrack.EndTime),
                        UploadMagnetometerData(lastTrack.EndTime),
                        UploadLinearAccelerationData(lastTrack.EndTime),
                        UploadGravityData(lastTrack.EndTime),
                        UploadOrientationData(lastTrack.EndTime),
                        UploadQuaternionData(lastTrack.EndTime)
                        );


                    var removeOldDataSuccess = await RemoveOldSensorData(lastTrack.EndTime, uploadSuccess);
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
        private bool RemoveOldTracks(DateTimeOffset endTime)
        {
            try
            {
                Console.WriteLine("amount of tracks before upload: " + App.RepoManager.TrackEntryRepository.Count());


                List<TrackEntry> tracksWithChildren = new List<TrackEntry>();
                // get tracks before range
                List<TrackEntry> tracks = App.RepoManager.TrackEntryRepository.GetRangeByEndTime(DateTimeOffset.MinValue, endTime).ToList();
                foreach (var track in tracks)
                {
                    tracksWithChildren.Add(App.RepoManager.TrackEntryRepository.GetWithChildren(track.Id));
                }

                foreach (var trackWithChildren in tracksWithChildren)
                {
                    App.RepoManager.TrackEntryRepository.Remove(trackWithChildren);
                }


                Console.WriteLine("Amount of tracks after upload: " + App.RepoManager.TrackEntryRepository.Count());
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Log.Error(e.ToString());
                return false;
            }

        }

        private Task<bool[]> RemoveOldSensorData(DateTimeOffset timestamp, bool[] uploadSuccess)
        {
            // creating list of items where uploads are succeeded to be removed as a batch
            List<Task<bool>> toBeRemoved = new List<Task<bool>>();
            if (uploadSuccess[0]) {
                toBeRemoved.Add(RemoveOldSensorMeasurementSessions(timestamp));
            }
            if (uploadSuccess[1])
            {
                toBeRemoved.Add(RemoveOldAccelerometerData(timestamp));
            }
            if (uploadSuccess[2])
            {
                toBeRemoved.Add(RemoveOldGyroscopeData(timestamp));
            }
            if (uploadSuccess[3])
            {
                toBeRemoved.Add(RemoveOldMagnetometerData(timestamp));
            }
            if (uploadSuccess[4])
            {
                toBeRemoved.Add(RemoveOldLinearAccelerationData(timestamp));
            }
            if (uploadSuccess[5])
            {
                toBeRemoved.Add(RemoveOldGravityData(timestamp));
            }
            if (uploadSuccess[6])
            {
                toBeRemoved.Add(RemoveOldOrientationData(timestamp));
            }
            if (uploadSuccess[7])
            {
                toBeRemoved.Add(RemoveOldQuaternionData(timestamp));
            }
            
            var removeOldSensorDataSuccess = Task.WhenAll(
                        toBeRemoved
                    );
            return removeOldSensorDataSuccess;
        }

        private async Task<bool> RemoveOldSensorMeasurementSessions(DateTimeOffset timestamp)
        {
            ISensorMeasurementSessionRepository sensorMeasurementRepository = App.RepoManager.SensorMeasurementSessionRepository;
            return await Task.Run(() => sensorMeasurementRepository.RemoveRangeBeforeTimestamp(timestamp));
        }

        private async Task<bool> RemoveOldAccelerometerData(DateTimeOffset timestamp)
        {
            ISensorRepository<Accelerometer> accelerometerRepository = App.RepoManager.AccelerometerRepository;
            Console.WriteLine("amount of accelerometerdata before: " + accelerometerRepository.Count());
            var removedSuccessfully = await Task.Run(() => accelerometerRepository.RemoveRangeBeforeTimestamp(timestamp));
            if (removedSuccessfully)
            {
                Console.WriteLine("amount of accelerometerdata after: " + accelerometerRepository.Count());
            }

            return removedSuccessfully;
        }

        private async Task<bool> RemoveOldGyroscopeData(DateTimeOffset timestamp)
        {
            ISensorRepository<Gyroscope> gyroscopeRepository = App.RepoManager.GyroscopeRepository;

            return await Task.Run(() => gyroscopeRepository.RemoveRangeBeforeTimestamp(timestamp));
        }

        private async Task<bool> RemoveOldMagnetometerData(DateTimeOffset timestamp)
        {
            ISensorRepository<Magnetometer> magnetometerRepository = App.RepoManager.MagnetometerRepository;

            return await Task.Run(() => magnetometerRepository.RemoveRangeBeforeTimestamp(timestamp));
        }

        private async Task<bool> RemoveOldLinearAccelerationData(DateTimeOffset timestamp)
        {
            ISensorRepository<LinearAcceleration> linearAccelerationRepository = App.RepoManager.LinearAccelerationRepository;

            return await Task.Run(() => linearAccelerationRepository.RemoveRangeBeforeTimestamp(timestamp));
        }

        private async Task<bool> RemoveOldGravityData(DateTimeOffset timestamp)
        {
            ISensorRepository<Gravity> gravityRepository = App.RepoManager.GravityRepository;

            return await Task.Run(() => gravityRepository.RemoveRangeBeforeTimestamp(timestamp));
        }

        private async Task<bool> RemoveOldOrientationData(DateTimeOffset timestamp)
        {
            ISensorRepository<Orientation> orientationRepository = App.RepoManager.OrientationRepository;

            return await Task.Run(() => orientationRepository.RemoveRangeBeforeTimestamp(timestamp));
        }

        private async Task<bool> RemoveOldQuaternionData(DateTimeOffset timestamp)
        {
            ISensorRepository<Quaternion> quaternionRepository = App.RepoManager.QuaternionRepository;

            return await Task.Run(() => quaternionRepository.RemoveRangeBeforeTimestamp(timestamp));
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
                    Settings.Current.BatteryInfoLastUpload = batteryEntries.Last().Timestamp.Ticks;
                }
            }

            return true;
        }

        public async Task<bool> UploadAccelerometerData(DateTimeOffset endTime)
        {
            try
            {
                List<Accelerometer> accelerometerData = App.RepoManager.AccelerometerRepository.GetRange(DateTimeOffset.MinValue, endTime).ToList();

                bool success = await ApiClient.PostAccelerometerData(Settings.Current.Device, accelerometerData);

                if (!success)
                {
                    Log.Error($"Tried to send {accelerometerData.Count} accelerometerdata but failed");
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload accelerometerdata " + e);
                return false;
            }
        }

        public async Task<bool> UploadGyroscopeData(DateTimeOffset endTime)
        {
            try
            {
                //get gyroscopedata
                List<Gyroscope> gyroscopeData = App.RepoManager.GyroscopeRepository.GetRange(DateTimeOffset.MinValue, endTime).ToList();

                bool success = await ApiClient.PostGyroscopeData(Settings.Current.Device, gyroscopeData);

                if (!success)
                {
                    Log.Error($"Tried to send {gyroscopeData.Count} gyroscopedata but failed");
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload gyroscopedata " + e);
                return false;
            }
        }

        public async Task<bool> UploadMagnetometerData(DateTimeOffset endTime)
        {
            try
            {
                //get magnetometerdata
                List<Magnetometer> magnetometerData = App.RepoManager.MagnetometerRepository.GetRange(DateTimeOffset.MinValue, endTime).ToList();

                bool success = await ApiClient.PostMagnetometerData(Settings.Current.Device, magnetometerData);

                if (!success)
                {
                    Log.Error($"Tried to send {magnetometerData.Count} magnetometerdata but failed");
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload magnetometerdata " + e);
                return false;
            }
        }
        private async Task<bool> UploadQuaternionData(DateTimeOffset endTime)
        {
            try
            {
                List<Quaternion> quaternionData = App.RepoManager.QuaternionRepository.GetRange(DateTimeOffset.MinValue, endTime).ToList();

                bool success = await ApiClient.PostQuaternionData(Settings.Current.Device, quaternionData);

                if (!success)
                {
                    Log.Error($"Tried to send {quaternionData.Count} quaterniondata but failed");
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload quaterniondata " + e);
                return false;
            }
        }

        private async Task<bool> UploadOrientationData(DateTimeOffset endTime)
        {
            try
            {
                List<Orientation> orientationData = App.RepoManager.OrientationRepository.GetRange(DateTimeOffset.MinValue, endTime).ToList();

                bool success = await ApiClient.PostOrientationData(Settings.Current.Device, orientationData);

                if (!success)
                {
                    Log.Error($"Tried to send {orientationData.Count} orientationdata but failed");
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload orientation " + e);
                return false;
            }
        }

        private async Task<bool> UploadGravityData(DateTimeOffset endTime)
        {
            try
            {
                List<Gravity> gravityData = App.RepoManager.GravityRepository.GetRange(DateTimeOffset.MinValue, endTime).ToList();

                bool success = await ApiClient.PostGravityData(Settings.Current.Device, gravityData);

                if (!success)
                {
                    Log.Error($"Tried to send {gravityData.Count} gravitydata but failed");
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload gravitydata " + e);
                return false;
            }
        }

        private async Task<bool> UploadLinearAccelerationData(DateTimeOffset endTime)
        {
            try
            {
                List<LinearAcceleration> linearAccelerationData = App.RepoManager.LinearAccelerationRepository.GetRange(DateTimeOffset.MinValue, endTime).ToList();

                bool success = await ApiClient.PostLinearAccelerationData(Settings.Current.Device, linearAccelerationData);

                if (!success)
                {
                    Log.Error($"Tried to send {linearAccelerationData.Count} linearaccelerationdata but failed");
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload linearaccelerationdata " + e);
                return false;
            }
        }

        public async Task<bool> UploadSensorMeasurementSessions(DateTimeOffset endTime)
        {
            try
            {
                //get sensormeasurementsessiondata
                List<SensorMeasurementSession> sensorMeasurementSessions = App.RepoManager.SensorMeasurementSessionRepository.GetRange(DateTimeOffset.MinValue, endTime).ToList();

                bool success = await ApiClient.PostSensorMeasurementSessions(Settings.Current.Device, sensorMeasurementSessions);

                if (!success)
                {
                    Log.Error($"Tried to send {sensorMeasurementSessions.Count} sensormeasurementsessions but failed");
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload sensormeasurementsessions " + e);
                return false;
            }
        }

        public async Task<bool> UploadTracks(DateTimeOffset endTime)
        {
            try
            {
                List<TrackEntry> trackEntriesWithChildren = new List<TrackEntry>();
                List<TrackEntry> trackEntries = App.RepoManager.TrackEntryRepository.GetRangeByEndTime(DateTimeOffset.MinValue, endTime).ToList();
                //get the models with children
                foreach (var track in trackEntries)
                { 
                    trackEntriesWithChildren.Add(App.RepoManager.TrackEntryRepository.GetWithChildren(track.Id));
                }

                // convert to trackDTO
                List<TabiApiClient.Models.TrackEntry> trackDTO = new List<TabiApiClient.Models.TrackEntry>();
                foreach (var trackEntryWithChildren in trackEntriesWithChildren)
                {
                    List<TabiApiClient.Models.TransportationMode> transportationModes = new List<TabiApiClient.Models.TransportationMode>();
                    foreach (var transportationMode in trackEntryWithChildren.TransportationModes)
                    {
                        transportationModes.Add(new TabiApiClient.Models.TransportationMode() {Id = (int)transportationMode.Mode,Mode = transportationMode.Mode.ToString() });
                    }

                    trackDTO.Add(new TabiApiClient.Models.TrackEntry()
                    {
                        Id = trackEntryWithChildren.Id,
                        StartTime = trackEntryWithChildren.StartTime,
                        EndTime = trackEntryWithChildren.EndTime,
                        TransportationModes = transportationModes
                    });
                }

                bool success = await ApiClient.PostTrackEntries(Settings.Current.Device, trackDTO);

                if (!success)
                {
                    Log.Error($"Tried to send {trackEntries.Count} trackEntries but failed");
                    return false;
                }


                

                return true;
            } catch (Exception e)
            {
                Log.Error("Could not upload tracks " + e);
                return false;
            }

        }
    }
}
