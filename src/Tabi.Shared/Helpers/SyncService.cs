using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Tabi.DataObjects;
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
                await Login();
                await UploadPositions();
                await UploadLogs();
                await UploadBatteryInfo();
                await UploadStopVisits();

                //sensordata
                var success = await UploadTracks();
                if (success)
                {
                    var uploadSuccess = await Task.WhenAll(
                        UploadSensorMeasurementSessions(),
                        UploadAccelerometerData(),
                        UploadGyroscopeData(),
                        UploadMagnetometerData(),
                        UploadLinearAccelerationData(),
                        UploadGravityData(),
                        UploadOrientationData(),
                        UploadQuaternionData()
                    );
                    foreach (var item in uploadSuccess)
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

        public async Task<bool> UploadAccelerometerData()
        {
            try
            {
                List<Accelerometer> accelerometerData = App.RepoManager.AccelerometerRepository.GetAll().ToList();

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

        public async Task<bool> UploadGyroscopeData()
        {
            try
            {
                //get gyroscopedata
                List<Gyroscope> gyroscopeData = App.RepoManager.GyroscopeRepository.GetAll().ToList();

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

        public async Task<bool> UploadMagnetometerData()
        {
            try
            {
                //get magnetometerdata
                List<Magnetometer> magnetometerData = App.RepoManager.MagnetometerRepository.GetAll().ToList();

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
        private async Task<bool> UploadQuaternionData()
        {
            try
            {
                List<Quaternion> quaternionData = App.RepoManager.QuaternionRepository.GetAll().ToList();

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

        private async Task<bool> UploadOrientationData()
        {
            try
            {
                List<Orientation> orientationData = App.RepoManager.OrientationRepository.GetAll().ToList();

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

        private async Task<bool> UploadGravityData()
        {
            try
            {
                List<Gravity> gravityData = App.RepoManager.GravityRepository.GetAll().ToList();

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

        private async Task<bool> UploadLinearAccelerationData()
        {
            try
            {
                List<LinearAcceleration> linearAccelerationData = App.RepoManager.LinearAccelerationRepository.GetAll().ToList();

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

        public async Task<bool> UploadSensorMeasurementSessions()
        {
            try
            {
                //get sensormeasurementsessiondata
                List<SensorMeasurementSession> sensorMeasurementSessions = App.RepoManager.SensorMeasurementSessionRepository.GetAll().ToList();

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

        public async Task<bool> UploadTracks()
        {
            try
            {
                List<TrackEntry> trackEntries = App.RepoManager.TrackEntryRepository.GetAll().ToList();
                bool success = await ApiClient.PostTrackEntries(Settings.Current.Device, trackEntries);

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
