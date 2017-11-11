using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Tabi.DataObjects;
using TabiApiClient;

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
                await UploadAll(wifiOnly);
                lastAutoUpload = DateTimeOffset.Now;
            }
        }


        public async Task UploadAll(bool wifiOnly = true)
        {
            var wifi = Plugin.Connectivity.Abstractions.ConnectionType.WiFi;
            var connectionTypes = CrossConnectivity.Current.ConnectionTypes;

            if (!wifiOnly || connectionTypes.Contains(wifi))
            {
                await UploadPositions();
                await UploadLogs();
                await UploadBatteryInfo();
            }

        }

        public async Task UploadPositions()
        {
            DateTimeOffset lastUpload = DateTimeOffset.FromUnixTimeMilliseconds(Settings.Current.PositionLastUpload);
            List<PositionEntry> positions = App.RepoManager.PositionEntryRepository.After(lastUpload);
            if (positions.Count() > 0)
            {
                await Login();
                bool success = await ApiClient.PostPositions(Settings.Current.Device, positions);
                if (!success)
                {
                    Log.Error("Could not send positions");
                    return;
                }
                Settings.Current.PositionLastUpload = positions.Last().Timestamp.ToUnixTimeMilliseconds();
            }

        }


        public async Task<bool> UploadLogs()
        {
            DateTimeOffset lastUpload = DateTimeOffset.FromUnixTimeMilliseconds(Settings.Current.LogsLastUpload);

            List<LogEntry> logs = App.RepoManager.LogEntryRepository.After(lastUpload);
            if (logs.Count() > 0)
            {
                await Login();
                bool success = await ApiClient.PostLogs(Settings.Current.Device, logs);
                if (!success)
                {
                    Log.Error("Could not send logs");
                    return false;
                }
                Settings.Current.BatteryInfoLastUpload = logs.Last().Timestamp.ToUnixTimeMilliseconds();
                App.RepoManager.LogEntryRepository.ClearLogsBefore(logs.Last().Timestamp);
            }
            return true;
        }

        public async Task<bool> UploadBatteryInfo()
        {
            DateTimeOffset lastUpload = DateTimeOffset.FromUnixTimeMilliseconds(Settings.Current.BatteryInfoLastUpload);
            List<BatteryEntry> batteryEntries = App.RepoManager.BatteryEntryRepository.After(lastUpload);
            if(batteryEntries.Count() > 0)
            {
                await Login();
                bool success = await ApiClient.PostBatteryData(Settings.Current.Device, batteryEntries);
                if (!success)
                {
                    Log.Error("Could not send batterydata");
                    return false;
                }
                Settings.Current.BatteryInfoLastUpload = batteryEntries.Last().Timestamp.ToUnixTimeMilliseconds();
            }
           
            return true;
        }

    }
}
