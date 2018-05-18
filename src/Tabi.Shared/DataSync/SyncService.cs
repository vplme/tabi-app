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
using Tabi.Shared.DataSync;

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

        private ISyncTimeKeeper _timeKeeper;
        public ISyncTimeKeeper TimeKeeper
        {
            get
            {
                if (_timeKeeper == null)
                {
                    _timeKeeper = new SyncTimeKeeper(_repoManager);
                }

                return _timeKeeper;
            }
            set
            {
                _timeKeeper = value ?? throw new ArgumentNullException(nameof(value));
            }
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

                List<Task> toBeUploaded = new List<Task> {
                    UploadPositions(GatherPositions()),
                    UploadLogs(GatherLogs()),
                    UploadStopVisits(GatherStopVisits()),
                    UploadStops(GatherStops()),
                    UploadBatteryInfo(GatherBatteryEntries()),
                    UploadStopMotives(GatherStopMotives()),

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
                await Task.WhenAll(toBeUploaded);
                Log.Info($"Total upload took: {timer.EndAndReturnTime()}");

            }
            await ValidateCounts();
        }

        public List<PositionEntry> GatherPositions()
        {
            DateTimeOffset lastUpload = TimeKeeper.GetPreviousDone(UploadType.PositionEntry);
            return _repoManager.PositionEntryRepository.After(lastUpload);
        }


        public async Task<bool> UploadPositions(List<PositionEntry> positions)
        {
            bool success = false;
            if (positions.Any())
            {
                success = await _apiClient.PostPositions(Settings.Current.Device, positions);
                if (success)
                {
                    TimeKeeper.SetDone(UploadType.PositionEntry, positions.Last().Timestamp, positions.Count);
                }
                else
                {
                    Log.Error("Could not send positions");
                }
            }

            return success;
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
            var data = GatherAccelerometerData();
            await UploadAccelerometerData(data);
            await RemoveOldAccelerometerData();
        }

        private async Task UploadAndRemoveGyroscopeData()
        {
            var data = await Task.Run(() => GatherGyroscopeData());
            await UploadGyroscopeData(data);
            await RemoveOldGyroscopeData();
        }

        private async Task UploadAndRemoveMagnetometerData()
        {
            var data = await Task.Run(() => GatherMagnetometerData());

            await UploadMagnetometerData(data);
            await RemoveOldMagnetometerData();
        }

        private async Task UploadAndRemoveQuaternionData()
        {
            var data = await Task.Run(() => GatherQuaternionData());

            await UploadQuaternionData(data);
            await RemoveOldQuaternionData();
        }

        private async Task UploadAndRemoveOrientationData()
        {
            var data = await Task.Run(() => GatherOrientationData());

            await UploadOrientationData(data);
            await RemoveOldOrientationData();
        }

        private async Task UploadAndRemoveGravityData()
        {
            var data = await Task.Run(() => GatherGravityData());

            await UploadGravityData(data);
            await RemoveOldGravityData();
        }

        private async Task UploadAndRemoveLinearAcceleration()
        {
            var data = await Task.Run(() => GatherLinearAccelerationData());

            await UploadLinearAccelerationData(data);
            await RemoveOldLinearAccelerationData();
        }

        private async Task UploadAndRemoveSensorMeasurementSessions()
        {
            var data = await Task.Run(() => GatherSensorMeasurementSessions());

            await UploadSensorMeasurementSessions(data);
            await RemoveOldSensorMeasurementSessions();
        }

        private async Task UploadAndRemoveTransporationModes()
        {
            var data = await Task.Run(() => GatherTransportationModes());

            await UploadTransportationModes(data);
            // Donot remove
        }

        private async Task UploadAndRemoveTracks()
        {
            var data = await Task.Run(() => GatherTracks());

            await UploadTracks(data);
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

        public List<LogEntry> GatherLogs()
        {
            DateTimeOffset lastUpload = TimeKeeper.GetPreviousDone(UploadType.LogEntry);
            return _repoManager.LogEntryRepository.After(lastUpload);
        }

        public async Task<bool> UploadLogs(List<LogEntry> logs)
        {
            bool success = false;

            if (logs.Any())
            {
                List<TabiApiClient.Models.Log> apiLogs = logs.Select(l => l.ToApiModel()).ToList();
                try
                {
                    success = await _apiClient.PostLogs(Settings.Current.Device, apiLogs);
                }
                catch (Exception e)
                {
                    Log.Error("Failed to upload logs: " + e);
                }
                if (success)
                {
                    Settings.Current.LogsLastUpload = logs.Last().Timestamp.Ticks;
                    TimeKeeper.SetDone(UploadType.LogEntry, logs.Last().Timestamp, logs.Count);
                }
                else
                {
                    Log.Error("Could not send logs");

                }
            }
            return success;
        }

        public List<StopVisit> GatherStopVisits()
        {
            DateTimeOffset lastUpload = TimeKeeper.GetPreviousDone(UploadType.StopVisit);

            return _repoManager.StopVisitRepository.After(lastUpload).ToList();
        }

        public async Task<bool> UploadStopVisits(List<StopVisit> stopVisits)
        {
            bool success = false;

            List<TabiApiClient.Models.StopVisit> apiStopVisits = stopVisits.Select(s => s.ToApiModel()).ToList();

            success = await _apiClient.PostStopVisits(Settings.Current.Device, apiStopVisits);
            if (success)
            {
                TimeKeeper.SetDone(UploadType.StopVisit, apiStopVisits.Last().EndTimestamp, apiStopVisits.Count);

            }
            else
            {
                Log.Error("Could not send stopvisits");
            }

            return success;
        }

        public List<Stop> GatherStops()
        {
            DateTimeOffset lastUpload = TimeKeeper.GetPreviousDone(UploadType.Stop);
            return _repoManager.StopRepository.After(lastUpload);
        }

        public async Task<bool> UploadStops(List<Stop> stops)
        {
            bool success = false;

            List<TabiApiClient.Models.Stop> apiStops = stops.Select(s => s.ToApiModel()).ToList();

            if (apiStops.Any())
            {
                success = await _apiClient.PostStops(Settings.Current.Device, apiStops);
            }
            if (success)
            {
                TimeKeeper.SetDone(UploadType.Stop, apiStops.Last().Timestamp, apiStops.Count);
            }
            else
            {
                Log.Error("Could not send stops");
            }

            return success;
        }

        public List<Motive> GatherStopMotives()
        {
            DateTimeOffset lastUpload = TimeKeeper.GetPreviousDone(UploadType.StopMotive);
            return _repoManager.MotiveRepository.After(lastUpload).ToList();
        }

        public async Task<bool> UploadStopMotives(List<Motive> motives)
        {
            bool success = false;

            List<TabiApiClient.Models.UserStopMotive> apiStopMotives = motives.Select(s => s.ToUserStopMotiveApiModel()).ToList();
            if (motives.Any())
            {
                success = await _apiClient.PostUserStopMotives(Settings.Current.Device, apiStopMotives);
            }
            if (success)
            {
                TimeKeeper.SetDone(UploadType.StopMotive, apiStopMotives.Last().Timestamp, apiStopMotives.Count);
            }
            else
            {
                Log.Error("Could not send stops");
            }

            return success;
        }

        public List<BatteryEntry> GatherBatteryEntries()
        {
            DateTimeOffset lastUpload = TimeKeeper.GetPreviousDone(UploadType.BatteryEntry);

            return _repoManager.BatteryEntryRepository.After(lastUpload);
        }

        public async Task<bool> UploadBatteryInfo(List<BatteryEntry> batteryEntries)
        {
            bool success = false;

            List<TabiApiClient.Models.BatteryInfo> apiBatteryInfos = batteryEntries.Select(s => s.ToApiModel()).ToList();

            if (apiBatteryInfos.Any())
            {
                success = await _apiClient.PostBatteryData(Settings.Current.Device, apiBatteryInfos);
            }

            if (success)
            {
                TimeKeeper.SetDone(UploadType.BatteryEntry, batteryEntries.Last().Timestamp, batteryEntries.Count);

                Settings.Current.BatteryInfoLastUpload = batteryEntries.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
            }
            else
            {
                Log.Error($"Tried to send {batteryEntries.Count()} batterydata but failed");
            }


            return success;
        }

        public List<Accelerometer> GatherAccelerometerData()
        {
            DateTimeOffset lastUpload = TimeKeeper.GetPreviousDone(UploadType.Accelerometer);
            return _repoManager.AccelerometerRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();
        }

        public async Task<bool> UploadAccelerometerData(List<Accelerometer> accelerometerData)
        {
            bool success = false;

            try
            {
                if (accelerometerData.Any())
                {
                    success = await _apiClient.PostAccelerometerData(Settings.Current.Device, accelerometerData);
                }

                if (success)
                {
                    TimeKeeper.SetDone(UploadType.Accelerometer, accelerometerData.Last().Timestamp, accelerometerData.Count);

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

        public List<Gyroscope> GatherGyroscopeData()
        {
            DateTimeOffset lastUpload = TimeKeeper.GetPreviousDone(UploadType.Gyroscope);

            // TODO use after
            return _repoManager.GyroscopeRepository.GetRange(lastUpload.AddMilliseconds(1), DateTimeOffset.MaxValue).ToList();
        }

        public async Task<bool> UploadGyroscopeData(List<Gyroscope> gyroscopeData)
        {
            bool success = false;

            try
            {
                if (gyroscopeData.Any())
                {
                    success = await _apiClient.PostGyroscopeData(Settings.Current.Device, gyroscopeData);
                }

                if (success)
                {
                    TimeKeeper.SetDone(UploadType.Gyroscope, gyroscopeData.Last().Timestamp, gyroscopeData.Count);

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

        public List<Magnetometer> GatherMagnetometerData()
        {
            DateTimeOffset lastUpload = TimeKeeper.GetPreviousDone(UploadType.Magnetometer);
            return _repoManager.MagnetometerRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();
        }

        public async Task<bool> UploadMagnetometerData(List<Magnetometer> magnetometerData)
        {
            bool success = false;

            try
            {
                if (magnetometerData.Any())
                {
                    success = await _apiClient.PostMagnetometerData(Settings.Current.Device, magnetometerData);
                }

                if (success)
                {
                    TimeKeeper.SetDone(UploadType.Magnetometer, magnetometerData.Last().Timestamp, magnetometerData.Count);

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

        public List<Quaternion> GatherQuaternionData()
        {
            DateTimeOffset lastUpload = TimeKeeper.GetPreviousDone(UploadType.Quaternion);
            return _repoManager.QuaternionRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

        }


        private async Task<bool> UploadQuaternionData(List<Quaternion> quaternionData)
        {
            bool success = false;

            try
            {
                success = await _apiClient.PostQuaternionData(Settings.Current.Device, quaternionData);

                if (success)
                {
                    TimeKeeper.SetDone(UploadType.Quaternion, quaternionData.Last().Timestamp, quaternionData.Count);

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

        public List<Orientation> GatherOrientationData()
        {
            DateTimeOffset lastUpload = TimeKeeper.GetPreviousDone(UploadType.Orientation);
            return _repoManager.OrientationRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();
        }

        private async Task<bool> UploadOrientationData(List<Orientation> orientationData)
        {
            bool success = false;

            try
            {
                success = await _apiClient.PostOrientationData(Settings.Current.Device, orientationData);

                if (success)
                {
                    TimeKeeper.SetDone(UploadType.Orientation, orientationData.Last().Timestamp, orientationData.Count);

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

        public List<Gravity> GatherGravityData()
        {
            DateTimeOffset lastUpload = TimeKeeper.GetPreviousDone(UploadType.Gravity);
            return _repoManager.GravityRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

        }

        private async Task<bool> UploadGravityData(List<Gravity> gravityData)
        {
            bool success = false;

            try
            {
                success = await _apiClient.PostGravityData(Settings.Current.Device, gravityData);

                if (success)
                {
                    TimeKeeper.SetDone(UploadType.Gravity, gravityData.Last().Timestamp, gravityData.Count);


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

        public List<LinearAcceleration> GatherLinearAccelerationData()
        {
            DateTimeOffset lastUpload = TimeKeeper.GetPreviousDone(UploadType.LinearAcceleration);
            return _repoManager.LinearAccelerationRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

        }

        private async Task<bool> UploadLinearAccelerationData(List<LinearAcceleration> linearAccelerationData)
        {
            bool success = false;

            try
            {

                if (linearAccelerationData.Any())
                {
                    success = await _apiClient.PostLinearAccelerationData(Settings.Current.Device, linearAccelerationData);
                }

                if (success)
                {
                    TimeKeeper.SetDone(UploadType.LinearAcceleration, linearAccelerationData.Last().Timestamp, linearAccelerationData.Count);

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

        public List<SensorMeasurementSession> GatherSensorMeasurementSessions()
        {
            DateTimeOffset lastUpload = TimeKeeper.GetPreviousDone(UploadType.SensorMeasurementSession);
            return _repoManager.SensorMeasurementSessionRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();
        }

        public async Task<bool> UploadSensorMeasurementSessions(List<SensorMeasurementSession> sensorMeasurementSessions)
        {
            bool success = false;

            try
            {
                success = await _apiClient.PostSensorMeasurementSessions(Settings.Current.Device, sensorMeasurementSessions);

                if (success)
                {
                    TimeKeeper.SetDone(UploadType.SensorMeasurementSession, sensorMeasurementSessions.Last().Timestamp, sensorMeasurementSessions.Count);


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

        public List<TransportationModeEntry> GatherTransportationModes()
        {
            DateTimeOffset lastUpload = TimeKeeper.GetPreviousDone(UploadType.TransportationMode);
            return _repoManager.TransportationModeRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();
        }

        public async Task<bool> UploadTransportationModes(List<TransportationModeEntry> transportationModeEntries)
        {
            bool success = false;
            try
            {
                IEnumerable<TabiApiClient.Models.TransportationMode> apiModels = null;
                if (transportationModeEntries.Any())
                {
                    apiModels = transportationModeEntries.Select(entry => entry.ToApiModel());

                    success = await _apiClient.PostTransportationModes(Settings.Current.Device, apiModels);
                }

                if (success)
                {
                    TimeKeeper.SetDone(UploadType.TransportationMode, apiModels.Last().Timestamp, apiModels.Count());

                    Settings.Current.TransportModeLastUpload = transportationModeEntries.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                }
                else
                {
                    Log.Error($"Tried to send {transportationModeEntries?.Count()} transportationModes but failed");
                }

                return success;
            }
            catch (Exception e)
            {
                Log.Error("Could not upload transportationModes " + e);
                return false;
            }
        }

        public List<TrackEntry> GatherTracks()
        {
            DateTimeOffset lastUpload = TimeKeeper.GetPreviousDone(UploadType.TrackEntry);
            return _repoManager.TrackEntryRepository.GetRangeByEndTime(lastUpload, DateTimeOffset.MaxValue).ToList();
        }

        public async Task<bool> UploadTracks(List<TrackEntry> trackEntries)
        {

            bool success = false;
            try
            {
                //gets tracks that are completed and between lastuploadtime and LastCompletedTrackEntry
                trackEntries.Remove(trackEntries.Last());

                // convert to trackDTO
                List<TabiApiClient.Models.TrackEntry> apiModels = trackEntries.Select(entry => entry.ToApiModel()).ToList();

                success = await _apiClient.PostTrackEntries(Settings.Current.Device, apiModels);

                if (success)
                {
                    TimeKeeper.SetDone(UploadType.TrackEntry, apiModels.Last().EndTime, apiModels.Count);

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
