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
                await UploadTracks();
                //await UploadSensorMeasurementSessions();
                //await UploadAccelerometerData();
                //await UploadGyroscopeData();
                //await UploadMagnetometerData();
                

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

        public async Task<bool> UploadAccelerometerData(List<Accelerometer> accelerometerData, int trackId)
        {
            try
            {
                //cluster accelerometerData per track


                bool success = await ApiClient.PostAccelerometerData(Settings.Current.Device, trackId, accelerometerData);

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

        public async Task<bool> UploadGyroscopeData(List<Gyroscope> gyroscopeData, int trackId)
        {
            try
            {
                //get gyroscopedata
                bool success = await ApiClient.PostGyroscopeData(Settings.Current.Device, trackId, gyroscopeData);

                if (!success)
                {
                    Log.Error($"Tried to send {gyroscopeData.Count} accelerometerdata but failed");
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

        public async Task<bool> UploadMagnetometerData(List<Magnetometer> magnetometerData, int trackId)
        {
            try
            {
                //get magnetometerdata
                bool success = await ApiClient.PostMagnetometerData(Settings.Current.Device, trackId, magnetometerData);

                if (!success)
                {
                    Log.Error($"Tried to send {magnetometerData.Count} accelerometerdata but failed");
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

        public async Task<bool> UploadSensorMeasurementSessions(List<SensorMeasurementSession> sensorMeasurementSessions, int trackId)
        {
            try
            {
                //get sensormeasurementsessiondata
                bool success = await ApiClient.PostSensorMeasurementSessions(Settings.Current.Device, trackId, sensorMeasurementSessions);

                if (!success)
                {
                    Log.Error($"Tried to send {sensorMeasurementSessions.Count} accelerometerdata but failed");
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

                // loop through tracks
                foreach (var track in trackEntries)
                {
                    //upload track and retrieve inserted id
                    var id = ApiClient.PostTrackEntries(Settings.Current.Device, track).Result;

                    if (id == 0)
                    {
                        Log.Error($"Tried to send {track.Id} accelerometerdata but failed");
                        return false;
                    }

                    //GET sensordata corresponds to track
                    List<SensorMeasurementSession> sensorMeasurementSessions = App.RepoManager.SensorMeasurementSessionRepository.GetRange(track.StartTime, track.EndTime).ToList();
                    await UploadSensorMeasurementSessions(sensorMeasurementSessions, id);

                    List<Accelerometer> accelerometerData = App.RepoManager.AccelerometerRepository.GetRange(track.StartTime, track.EndTime).ToList();
                    await UploadAccelerometerData(accelerometerData, id);

                    List<Gyroscope> gyroscopeData = App.RepoManager.GyroscopeRepository.GetRange(track.StartTime, track.EndTime).ToList();
                    await UploadGyroscopeData(gyroscopeData, id);

                    List<Magnetometer> magnetometerData = App.RepoManager.MagnetometerRepository.GetRange(track.StartTime, track.EndTime).ToList();
                    await UploadMagnetometerData(magnetometerData, id);
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
