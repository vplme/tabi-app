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

        public SyncService()
        {
            ApiClient = new ApiClient();
        }

        public async Task Login()
        {
            await ApiClient.Authenticate(Settings.Current.Username, Settings.Current.Password);

        }

        public async Task AutoUpload(bool wifiOnly = true)
        {
            var wifi = Plugin.Connectivity.Abstractions.ConnectionType.WiFi;
            var connectionTypes = CrossConnectivity.Current.ConnectionTypes;

            if (!wifiOnly || connectionTypes.Contains(wifi))
            {
                await UploadPositions();
            }

        }

        public async Task UploadPositions()
        {
            DateTimeOffset lastUpload = DateTimeOffset.FromUnixTimeMilliseconds(Settings.Current.PositionLastUpload);
            List<PositionEntry> positions = App.RepoManager.PositionEntryRepository.After(lastUpload);
            await Login();
            await ApiClient.SendPositions(Settings.Current.Device, positions);

            Settings.Current.PositionLastUpload = positions.Last().Timestamp.ToUnixTimeMilliseconds();
        }


    }
}
