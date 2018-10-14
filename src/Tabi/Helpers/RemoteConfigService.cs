using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Tabi.Files.LocalFilesSupport;
using TabiApiClient;
using Xamarin.Essentials;

namespace Tabi.Helpers
{
    public class RemoteConfigService
    {
        private readonly ApiClient _apiClient;
        private readonly Action _updatedAction;

        public RemoteConfigService(ApiClient apiClient, Action hasBeenUpdated = null)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _updatedAction = hasBeenUpdated;
        }

        public async Task UpdateRemoteConfig(int deviceId)
        {
            bool available = await _apiClient.Ping();

            if (available && deviceId != 0)
            {
                await _apiClient.Authenticate(Settings.Current.Username, Settings.Current.Password);

                string config = await _apiClient.GetDeviceConfig(deviceId);
                WriteConfiguration(config);
                if (_updatedAction != null)
                {
                    _updatedAction.Invoke();
                }
            }
        }

        public static IConfiguration LoadStoredConfiguration()
        {
            string fileName = GetStoredConfigurationpath();
            bool doesExist = File.Exists(fileName);

            IConfiguration config = null;
            if (doesExist)
            {
                try
                {
                    var builder = new ConfigurationBuilder().AddJsonFile(new LocalFileProvider(), fileName, false, false);
                    config = builder.Build();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return config;
        }

        private static string GetStoredConfigurationpath()
        {
            var mainDir = FileSystem.AppDataDirectory;
            return Path.Combine(mainDir, "stored_config.json");
        }

        private static void WriteConfiguration(string contents)
        {
            string fileName = GetStoredConfigurationpath();
            File.WriteAllText(fileName, contents);
        }



    }
}
