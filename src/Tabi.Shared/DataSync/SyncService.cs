using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.Shared.Extensions;
using Tabi.Shared.Helpers;
using TabiApiClient;
using TabiApiClient.Messages;

namespace Tabi.iOS.Helpers
{
    public class SyncService
    {
        private const int loginTimeout = 60;
        private readonly ApiClient _apiClient;
        private readonly IRepoManager _repoManager;
        private DateTimeOffset _lastAutoUpload;
        private DateTimeOffset _lastLogin;

        public SyncService(ApiClient apiClient, IRepoManager repoManager)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
        }

        public async Task Login()
        {
            await _apiClient.Authenticate(Settings.Current.Username, Settings.Current.Password);
        }

        public async Task AutoUpload(TimeSpan window, bool wifiOnly = true)
        {
            if (DateTimeOffset.Now - window >= _lastAutoUpload)
            {
                try
                {
                    await UploadAll(wifiOnly);
                    _lastAutoUpload = DateTimeOffset.Now;
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
                Timer timer = new Timer();
                timer.Start();
                //if (_lastLogin > DateTimeOffset.Now.AddMinutes(loginTimeout))
                //{
                await Login();
                //_lastLogin = DateTimeOffset.Now;
                //}

                Log.Info($"Login took: {timer.EndAndReturnTime()}");

                List<Task> toBeUploaded = new List<Task>
                {
                    UploadPositions(),
                    UploadLogs(),
                    UploadStopVisits(),
                    UploadBatteryInfo(),

                    UploadAndRemoveTracks(),
                    UploadAndRemoveGravityData(),
                    UploadAndRemoveGyroscopeData(),
                    UploadAndRemoveQuaternionData(),
                    UploadAndRemoveOrientationData(),
                    UploadAndRemoveMagnetometerData(),
                    UploadAndRemoveAccelerometerAsync(),
                    UploadAndRemoveLinearAcceleration(),
                    UploadAndRemoveTransporationModes(),
                    UploadAndRemoveSensorMeasurementSessions()
                };

                timer.Start();
                await Task.WhenAll(
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
                Log.Info($"Total upload took: {timer.EndAndReturnTime()}");

            }
            await ValidateCounts();
        }


        public async Task UploadPositions()
        {
            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.PositionLastUpload, TimeSpan.Zero);
            List<PositionEntry> positions = _repoManager.PositionEntryRepository.After(lastUpload);
            if (positions.Count() > 0)
            {
                bool success = await _apiClient.PostPositions(Settings.Current.Device, positions);
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
                List<TrackEntry> tracks = _repoManager.TrackEntryRepository.GetRangeByEndTime(DateTimeOffset.MinValue, DateTimeOffset.Now.AddDays(amountOfDaysAgo)).ToList();
                foreach (var track in tracks)
                {
                    tracksWithChildren.Add(_repoManager.TrackEntryRepository.GetWithChildren(track.Id));
                }

                foreach (var trackWithChildren in tracksWithChildren)
                {
                    _repoManager.TrackEntryRepository.Remove(trackWithChildren);
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
        private async Task<bool> RemoveOldSensorMeasurementSessions()
        {
            ISensorMeasurementSessionRepository sensorMeasurementRepository = _repoManager.SensorMeasurementSessionRepository;
            return await Task.Run(() => sensorMeasurementRepository.RemoveRangeBeforeTimestamp(new DateTimeOffset(Settings.Current.SensorMeasurementSessionLastUpload, TimeSpan.Zero)));
        }

        private async Task<bool> RemoveOldAccelerometerData()
        {
            ISensorRepository<Accelerometer> accelerometerRepository = _repoManager.AccelerometerRepository;
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
            ISensorRepository<Gyroscope> gyroscopeRepository = _repoManager.GyroscopeRepository;

            return await Task.Run(() => gyroscopeRepository.RemoveRangeBeforeTimestamp(new DateTimeOffset(Settings.Current.GyroscopeLastUpload, TimeSpan.Zero)));
        }

        private async Task<bool> RemoveOldMagnetometerData()
        {
            ISensorRepository<Magnetometer> magnetometerRepository = _repoManager.MagnetometerRepository;

            return await Task.Run(() => magnetometerRepository.RemoveRangeBeforeTimestamp(new DateTimeOffset(Settings.Current.MagnetometerLastUpload, TimeSpan.Zero)));
        }

        private async Task<bool> RemoveOldLinearAccelerationData()
        {
            ISensorRepository<LinearAcceleration> linearAccelerationRepository = _repoManager.LinearAccelerationRepository;

            return await Task.Run(() => linearAccelerationRepository.RemoveRangeBeforeTimestamp(new DateTimeOffset(Settings.Current.LinearAccelerationLastUpload, TimeSpan.Zero)));
        }

        private async Task<bool> RemoveOldGravityData()
        {
            ISensorRepository<Gravity> gravityRepository = _repoManager.GravityRepository;

            return await Task.Run(() => gravityRepository.RemoveRangeBeforeTimestamp(new DateTimeOffset(Settings.Current.GravityLastUpload, TimeSpan.Zero)));
        }

        private async Task<bool> RemoveOldOrientationData()
        {
            ISensorRepository<Orientation> orientationRepository = _repoManager.OrientationRepository;

            return await Task.Run(() => orientationRepository.RemoveRangeBeforeTimestamp(new DateTimeOffset(Settings.Current.OrientationLastUpload, TimeSpan.Zero)));
        }

        private async Task<bool> RemoveOldQuaternionData()
        {
            ISensorRepository<Quaternion> quaternionRepository = _repoManager.QuaternionRepository;

            return await Task.Run(() => quaternionRepository.RemoveRangeBeforeTimestamp(new DateTimeOffset(Settings.Current.QuaternionLastUpload, TimeSpan.Zero)));
        }

        private async Task UploadAndRemoveAccelerometerAsync()
        {
            await UploadAccelerometerData();
            await RemoveOldAccelerometerData();
        }

        private async Task UploadAndRemoveGyroscopeData()
        {
            await UploadGyroscopeData();
            await RemoveOldGyroscopeData();
        }

        private async Task UploadAndRemoveMagnetometerData()
        {
            await UploadMagnetometerData();
            await RemoveOldMagnetometerData();
        }

        private async Task UploadAndRemoveQuaternionData()
        {
            await UploadQuaternionData();
            await RemoveOldQuaternionData();
        }

        private async Task UploadAndRemoveOrientationData()
        {
            await UploadOrientationData();
            await RemoveOldOrientationData();
        }

        private async Task UploadAndRemoveGravityData()
        {
            await UploadGravityData();
            await RemoveOldGravityData();
        }

        private async Task UploadAndRemoveLinearAcceleration()
        {
            await UploadLinearAccelerationData();
            await RemoveOldLinearAccelerationData();
        }

        private async Task UploadAndRemoveSensorMeasurementSessions()
        {
            await UploadSensorMeasurementSessions();
            await RemoveOldSensorMeasurementSessions();
        }

        private async Task UploadAndRemoveTransporationModes()
        {
            await UploadTransportationModes();
            // Donot remove
        }

        private async Task UploadAndRemoveTracks()
        {
            await UploadTracks();
            // dont remove tracks
        }

        public async Task<bool> ValidateCounts()
        {
            bool valid = true;

            DateTimeOffset lastPositionUploaded = new DateTimeOffset(Settings.Current.PositionLastUpload, TimeSpan.Zero);
            DateTimeOffset lastLogUploaded = new DateTimeOffset(Settings.Current.LogsLastUpload, TimeSpan.Zero);
            DateTimeOffset lastBatteryInfoUploaded = new DateTimeOffset(Settings.Current.BatteryInfoLastUpload, TimeSpan.Zero);

            int positionCount = _repoManager.PositionEntryRepository.CountBefore(lastPositionUploaded);
            int logCount = _repoManager.LogEntryRepository.CountBefore(lastLogUploaded);
            int batteryCount = _repoManager.BatteryEntryRepository.CountBefore(lastBatteryInfoUploaded);

            DeviceCounts counts = await _apiClient.GetDeviceCounts(Settings.Current.Device);
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
            bool success = false;

            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.LogsLastUpload, TimeSpan.Zero);

            List<LogEntry> logs = _repoManager.LogEntryRepository.After(lastUpload);
            if (logs.Count() > 0)
            {
                try
                {
                    success = await _apiClient.PostLogs(Settings.Current.Device, logs);
                }
                catch (Exception e)
                {
                    Log.Error("Failed to upload logs: " + e);
                }
                if (success)
                {
                    Settings.Current.LogsLastUpload = logs.Last().Timestamp.Ticks;
                    //_repoManager.LogEntryRepository.ClearLogsBefore(logs.Last().Timestamp);

                }
                else
                {
                    Log.Error("Could not send logs");

                }
            }
            return success;
        }


        public async Task<bool> UploadStopVisits()
        {
            bool success = false;

            List<StopVisit> stopVisits = _repoManager.StopVisitRepository.GetAll().ToList();
            // Retrieve stops
            foreach (StopVisit sv in stopVisits)
            {
                sv.Stop = _repoManager.StopRepository.Get(sv.StopId);
            }

            success = await _apiClient.PostStopVisits(Settings.Current.Device, stopVisits);
            if (success)
            {
                // TODO On success
            }
            else
            {
                Log.Error("Could not send stopvisits");
            }

            return success;
        }

        public async Task<bool> UploadBatteryInfo()
        {
            bool success = false;

            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.BatteryInfoLastUpload, TimeSpan.Zero);
            List<BatteryEntry> batteryEntries = _repoManager.BatteryEntryRepository.After(lastUpload);
            if (batteryEntries.Any())
            {
                success = await _apiClient.PostBatteryData(Settings.Current.Device, batteryEntries);
                if (success)
                {
                    Settings.Current.BatteryInfoLastUpload = batteryEntries.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                }
                else
                {
                    Log.Error($"Tried to send {batteryEntries.Count()} batterydata but failed");
                }
            }

            return success;
        }

        public async Task<bool> UploadAccelerometerData()
        {
            bool success = false;

            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.AccelerometerLastUpload, TimeSpan.Zero);
            try
            {
                List<Accelerometer> accelerometerData = _repoManager.AccelerometerRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

                success = await _apiClient.PostAccelerometerData(Settings.Current.Device, accelerometerData);

                if (success)
                {
                    Settings.Current.AccelerometerLastUpload = accelerometerData.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                }
                else
                {
                    Log.Error($"Tried to send {accelerometerData.Count} accelerometerdata but failed");
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not upload accelerometerdata " + e);
            }

            return success;
        }

        public async Task<bool> UploadGyroscopeData()
        {
            bool success = false;

            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.GyroscopeLastUpload, TimeSpan.Zero);
            try
            {
                //get gyroscopedata
                List<Gyroscope> gyroscopeData = _repoManager.GyroscopeRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

                success = await _apiClient.PostGyroscopeData(Settings.Current.Device, gyroscopeData);

                if (success)
                {
                    Settings.Current.GyroscopeLastUpload = gyroscopeData.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                }
                else
                {
                    Log.Error($"Tried to send {gyroscopeData.Count} gyroscopedata but failed");
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not upload gyroscopedata " + e);
            }

            return success;
        }

        public async Task<bool> UploadMagnetometerData()
        {
            bool success = false;

            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.MagnetometerLastUpload, TimeSpan.Zero);
            try
            {
                //get magnetometerdata
                List<Magnetometer> magnetometerData = _repoManager.MagnetometerRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

                success = await _apiClient.PostMagnetometerData(Settings.Current.Device, magnetometerData);

                if (success)
                {
                    Settings.Current.MagnetometerLastUpload = magnetometerData.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                }
                else
                {
                    Log.Error($"Tried to send {magnetometerData.Count} magnetometerdata but failed");
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not upload magnetometerdata " + e);
            }

            return success;
        }
        private async Task<bool> UploadQuaternionData()
        {
            bool success = false;

            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.QuaternionLastUpload, TimeSpan.Zero);
            try
            {
                List<Quaternion> quaternionData = _repoManager.QuaternionRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

                success = await _apiClient.PostQuaternionData(Settings.Current.Device, quaternionData);

                if (success)
                {
                    Settings.Current.QuaternionLastUpload = quaternionData.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                }
                else
                {
                    Log.Error($"Tried to send {quaternionData.Count} quaterniondata but failed");
                }

            }
            catch (Exception e)
            {
                Log.Error("Could not upload quaterniondata " + e);
            }

            return success;
        }

        private async Task<bool> UploadOrientationData()
        {
            bool success = false;

            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.OrientationLastUpload, TimeSpan.Zero);
            try
            {
                List<Orientation> orientationData = _repoManager.OrientationRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

                success = await _apiClient.PostOrientationData(Settings.Current.Device, orientationData);

                if (success)
                {
                    Settings.Current.OrientationLastUpload = orientationData.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                }
                else
                {
                    Log.Error($"Tried to send {orientationData.Count} orientationdata but failed");
                }

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
            bool success = false;

            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.GravityLastUpload, TimeSpan.Zero);
            try
            {
                List<Gravity> gravityData = _repoManager.GravityRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

                success = await _apiClient.PostGravityData(Settings.Current.Device, gravityData);

                if (success)
                {
                    Settings.Current.GravityLastUpload = gravityData.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                }
                else
                {
                    Log.Error($"Tried to send {gravityData.Count} gravitydata but failed");
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not upload gravitydata " + e);
            }
            return success;
        }

        private async Task<bool> UploadLinearAccelerationData()
        {
            bool success = false;

            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.LinearAccelerationLastUpload, TimeSpan.Zero);

            try
            {
                List<LinearAcceleration> linearAccelerationData = _repoManager.LinearAccelerationRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

                if (linearAccelerationData.Any())
                {
                    success = await _apiClient.PostLinearAccelerationData(Settings.Current.Device, linearAccelerationData);
                }

                if (success)
                {
                    Settings.Current.LinearAccelerationLastUpload = linearAccelerationData.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                }
                else
                {
                    Log.Error($"Tried to send {linearAccelerationData.Count} linearaccelerationdata but failed");
                }

            }
            catch (Exception e)
            {
                Log.Error("Could not upload linearaccelerationdata " + e);
            }

            return success;
        }

        public async Task<bool> UploadSensorMeasurementSessions()
        {
            bool success = false;

            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.SensorMeasurementSessionLastUpload, TimeSpan.Zero);
            try
            {
                //get sensormeasurementsessiondata
                List<SensorMeasurementSession> sensorMeasurementSessions = _repoManager.SensorMeasurementSessionRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

                success = await _apiClient.PostSensorMeasurementSessions(Settings.Current.Device, sensorMeasurementSessions);

                if (success)
                {
                    Settings.Current.SensorMeasurementSessionLastUpload = sensorMeasurementSessions.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                }
                else
                {
                    Log.Error($"Tried to send {sensorMeasurementSessions.Count} sensormeasurementsessions but failed");

                }
            }
            catch (Exception e)
            {
                Log.Error("Could not upload sensormeasurementsessions " + e);
            }

            return success;
        }

        public async Task<bool> UploadTransportationModes()
        {
            bool success = false;

            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.TransportModeLastUpload, TimeSpan.Zero);

            try
            {
                IEnumerable<TabiApiClient.Models.TransportationMode> transportModes = null;

                //get transportmodes that are between lastuploaded and lasttrackentry
                List<TransportationModeEntry> transportationModeEntries = _repoManager.TransportationModeRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

                if (transportationModeEntries.Any())
                {
                    IEnumerable<TabiApiClient.Models.TransportationMode> apiModels = transportationModeEntries.Select(entry => entry.ToApiModel());

                    success = await _apiClient.PostTransportationModes(Settings.Current.Device, transportModes);
                }

                if (success)
                {
                    Settings.Current.TransportModeLastUpload = transportationModeEntries.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                }
                else
                {
                    Log.Error($"Tried to send {transportModes?.Count()} transportationModes but failed");
                }

                return success;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload transportationModes " + e);
                return false;
            }
        }

        public async Task<bool> UploadTracks()
        {
            DateTimeOffset lastUpload = new DateTimeOffset(Settings.Current.TracksLastUpload, TimeSpan.Zero);

            bool success = false;
            try
            {
                //gets tracks that are completed and between lastuploadtime and LastCompletedTrackEntry
                List<TrackEntry> trackEntries = _repoManager.TrackEntryRepository.GetRangeByEndTime(lastUpload, DateTimeOffset.MaxValue).ToList();
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

                success = await _apiClient.PostTrackEntries(Settings.Current.Device, trackDTO);

                if (success)
                {
                    Settings.Current.TracksLastUpload = trackEntries.Last().EndTime.Ticks;
                }
                else
                {
                    Log.Error($"Tried to send {trackEntries.Count} trackEntries but failed");
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not upload tracks " + e);
            }

            return success;

        }


    }
}
