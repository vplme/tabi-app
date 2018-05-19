﻿using System;
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

        public async Task<bool> Ping(int timeout)
        {
            return await _apiClient.Ping(timeout);
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
                IEnumerable<TabiApiClient.Models.PositionEntry> apiPositions = positions.Select(p => p.ToApiModel());
                try
                {
                    success = await _apiClient.PostPositions(Settings.Current.Device, apiPositions);
                }
                catch (Exception e)
                {
                    Log.Error("Failed to upload logs: " + e);
                }

                if (success)
                {
                    TimeKeeper.SetDone(UploadType.PositionEntry, positions.Last().Timestamp, positions.Count);
                }
                else
                {
                    Log.Error("Could not send positions");
                }
            }
            else
            {
                success = true;
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
                    TimeKeeper.SetDone(UploadType.LogEntry, logs.Last().Timestamp, logs.Count);
                }
                else
                {
                    Log.Error("Could not send logs");
                }
            }
            else
            {
                success = true;
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

            if (stopVisits.Any())
            {
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
            }
            else
            {
                success = true;
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

            if (stops.Any())
            {
                success = false;

                List<TabiApiClient.Models.Stop> apiStops = stops.Select(s => s.ToApiModel()).ToList();

                success = await _apiClient.PostStops(Settings.Current.Device, apiStops);

                if (success)
                {
                    TimeKeeper.SetDone(UploadType.Stop, apiStops.Last().Timestamp, apiStops.Count);
                }
                else
                {
                    Log.Error("Could not send stops");
                }

            }
            else
            {
                success = true;
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

            if (motives.Any())
            {

                List<TabiApiClient.Models.UserStopMotive> apiStopMotives = motives.Select(s => s.ToUserStopMotiveApiModel()).ToList();

                success = await _apiClient.PostUserStopMotives(Settings.Current.Device, apiStopMotives);

                if (success)
                {
                    TimeKeeper.SetDone(UploadType.StopMotive, apiStopMotives.Last().Timestamp, apiStopMotives.Count);
                }
                else
                {
                    Log.Error("Could not send stops");
                }
            }
            else
            {
                success = true;
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

                if (success)
                {
                    TimeKeeper.SetDone(UploadType.BatteryEntry, batteryEntries.Last().Timestamp, batteryEntries.Count);

                    Settings.Current.BatteryInfoLastUpload = batteryEntries.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                }
                else
                {
                    Log.Error($"Tried to send {batteryEntries.Count()} batterydata but failed");
                }
            }
            else
            {
                success = true;
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

            if (accelerometerData.Any())
            {
                IEnumerable<TabiApiClient.Models.MotionSensor> apiAccelerometerData = accelerometerData.Select(p => p.ToApiModel());

                try
                {
                    success = await _apiClient.PostAccelerometerData(Settings.Current.Device, apiAccelerometerData);
                }
                catch (Exception e)
                {
                    Log.Error("Could not upload accelerometerdata " + e);
                }

                if (success)
                {
                    TimeKeeper.SetDone(UploadType.Accelerometer, accelerometerData.OrderBy(x => x.Timestamp).Last().Timestamp, accelerometerData.Count);
                }
                else
                {
                    Log.Error($"Tried to send {accelerometerData.Count} accelerometerdata but failed");
                }
            }
            else
            {
                success = true;
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

            if (gyroscopeData.Any())
            {
                IEnumerable<TabiApiClient.Models.MotionSensor> apiGyroscopeData = gyroscopeData.Select(p => p.ToApiModel());

                try
                {

                    success = await _apiClient.PostGyroscopeData(Settings.Current.Device, apiGyroscopeData);
                }
                catch (Exception e)
                {
                    Log.Error("Could not upload gyroscopedata " + e);
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
            else
            {
                success = true;
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


            if (magnetometerData.Any())
            {
                IEnumerable<TabiApiClient.Models.MotionSensor> apiMagnetometerData = magnetometerData.Select(p => p.ToApiModel());

                try
                {
                    success = await _apiClient.PostMagnetometerData(Settings.Current.Device, apiMagnetometerData);
                }
                catch (Exception e)
                {
                    Log.Error("Could not upload magnetometerdata " + e);
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
            else
            {
                success = true;
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

            if (quaternionData.Any())
            {
                IEnumerable<TabiApiClient.Models.Quaternion> apiQuaternionData = quaternionData.Select(p => p.ToApiModel());

                try
                {
                    success = await _apiClient.PostQuaternionData(Settings.Current.Device, apiQuaternionData);
                }
                catch (Exception e)
                {
                    Log.Error("Could not upload quaterniondata " + e);
                }

                if (success)
                {
                    TimeKeeper.SetDone(UploadType.Quaternion, quaternionData.Last().Timestamp, quaternionData.Count);
                }
                else
                {
                    Log.Error($"Tried to send {quaternionData.Count} quaterniondata but failed");
                }
            }
            else
            {
                success = true;
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

            if (orientationData.Any())
            {
                IEnumerable<TabiApiClient.Models.MotionSensor> apiOrientationData = orientationData.Select(p => p.ToApiModel());
                try
                {
                    success = await _apiClient.PostOrientationData(Settings.Current.Device, apiOrientationData);
                }
                catch (Exception e)
                {
                    Log.Error("Could not upload orientation " + e);
                }
                if (success)
                {
                    TimeKeeper.SetDone(UploadType.Orientation, orientationData.Last().Timestamp, orientationData.Count);

                    Settings.Current.OrientationLastUpload = orientationData.OrderBy(x => x.Timestamp).Last().Timestamp.Ticks;
                }
                else
                {
                    Log.Error($"Tried to send {orientationData.Count} orientationdata but failed");
                }
            }
            else
            {
                success = true;
            }

            return success;

        }

        public List<Gravity> GatherGravityData()
        {
            DateTimeOffset lastUpload = TimeKeeper.GetPreviousDone(UploadType.Gravity);
            return _repoManager.GravityRepository.GetRange(lastUpload, DateTimeOffset.MaxValue).ToList();

        }

        private async Task<bool> UploadGravityData(List<Gravity> gravityData)
        {
            bool success = false;

            if (gravityData.Any())
            {
                IEnumerable<TabiApiClient.Models.MotionSensor> apiGravityData = gravityData.Select(p => p.ToApiModel());
                try
                {
                    success = await _apiClient.PostGravityData(Settings.Current.Device, apiGravityData);
                }
                catch (Exception e)
                {
                    Log.Error("Could not upload gravitydata " + e);
                }
                if (success)
                {
                    TimeKeeper.SetDone(UploadType.Gravity, gravityData.Last().Timestamp, gravityData.Count);
                }
                else
                {
                    Log.Error($"Tried to send {gravityData.Count} gravitydata but failed");
                }
            }
            else
            {
                success = true;
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

            IEnumerable<TabiApiClient.Models.MotionSensor> apiLinearAccData = linearAccelerationData.Select(p => p.ToApiModel());

            if (linearAccelerationData.Any())
            {
                try
                {
                    success = await _apiClient.PostLinearAccelerationData(Settings.Current.Device, apiLinearAccData);
                }
                catch (Exception e)
                {
                    Log.Error("Could not upload linearaccelerationdata " + e);
                }

                if (success)
                {
                    TimeKeeper.SetDone(UploadType.LinearAcceleration, linearAccelerationData.Last().Timestamp, linearAccelerationData.Count);
                }
                else
                {
                    Log.Error($"Tried to send {linearAccelerationData.Count} linearaccelerationdata but failed");
                }
            }
            else
            {
                success = true;
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

            if (sensorMeasurementSessions.Any())
            {
                IEnumerable<TabiApiClient.Models.SensorMeasurementSession> apiSensorSessions = sensorMeasurementSessions.Select(s => s.ToApiModel());

                try
                {
                    success = await _apiClient.PostSensorMeasurementSessions(Settings.Current.Device, apiSensorSessions);
                }
                catch (Exception e)
                {
                    Log.Error("Could not upload sensormeasurementsessions " + e);
                }
                if (success)
                {
                    TimeKeeper.SetDone(UploadType.SensorMeasurementSession, sensorMeasurementSessions.Last().Timestamp, sensorMeasurementSessions.Count);
                }
                else
                {
                    Log.Error($"Tried to send {sensorMeasurementSessions.Count} sensormeasurementsessions but failed");
                }
            }
            else
            {
                success = true;
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

            if (transportationModeEntries.Any())
            {
                IEnumerable<TabiApiClient.Models.TransportationMode> apiModels = transportationModeEntries.Select(entry => entry.ToApiModel());
                try
                {
                    success = await _apiClient.PostTransportationModes(Settings.Current.Device, apiModels);
                }
                catch (Exception e)
                {
                    Log.Error("Could not upload transportationModes " + e);
                    return false;
                }

                if (success)
                {
                    TimeKeeper.SetDone(UploadType.TransportationMode, apiModels.Last().Timestamp, apiModels.Count());
                }
                else
                {
                    Log.Error($"Tried to send {transportationModeEntries?.Count()} transportationModes but failed");
                }
            }
            else
            {
                success = true;
            }

            return success;

        }

        public List<TrackEntry> GatherTracks()
        {
            DateTimeOffset lastUpload = TimeKeeper.GetPreviousDone(UploadType.TrackEntry);
            return _repoManager.TrackEntryRepository.GetRangeByEndTime(lastUpload, DateTimeOffset.MaxValue).ToList();
        }

        public async Task<bool> UploadTracks(List<TrackEntry> trackEntries)
        {
            bool success = false;

            if (trackEntries.Any())
            {
                //gets tracks that are completed and between lastuploadtime and LastCompletedTrackEntry
                trackEntries.Remove(trackEntries.Last());

                // convert to trackDTO
                List<TabiApiClient.Models.TrackEntry> apiModels = trackEntries.Select(entry => entry.ToApiModel()).ToList();
                try
                {
                    success = await _apiClient.PostTrackEntries(Settings.Current.Device, apiModels);
                }
                catch (Exception e)
                {
                    Log.Error("Could not upload tracks " + e);
                }
                if (success)
                {
                    TimeKeeper.SetDone(UploadType.TrackEntry, trackEntries.Last().EndTime, apiModels.Count);

                }
                else
                {
                    Log.Error($"Tried to send {trackEntries.Count} trackEntries but failed");
                }
            }
            else
            {
                success = true;
            }

            return success;

        }
    }
}
