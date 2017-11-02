using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tabi.DataObjects;
using TabiApiClient.Messages;
using TabiApiClient.Models;

namespace TabiApiClient
{
    public class ApiClient
    {
        private static HttpClient client;

        public HttpClient MockHttpClient { get; set; }
        public bool Mock { get; set; } = false;

        private string apiLocation;
        const string apiRoot = "/api/v1";
        private string token;
        private string userId;

        public ApiClient(string apiLocation = "https://tabi.0x2a.site")
        {
            this.apiLocation = apiLocation;
            client = new HttpClient();

        }

        private string PrefixApiPath(string path)
        {
            return apiLocation + apiRoot + path;
        }

        private HttpClient Client
        {
            get
            {
                if (this.Mock)
                {
                    return this.MockHttpClient;
                }

                return ApiClient.client;
            }
        }

        private HttpContent SerializeObject(object o)
        {
            string content = JsonConvert.SerializeObject(o);
            return new StringContent(content, Encoding.UTF8, "application/json");
        }

        private HttpClientHandler GetCustomHandler()
        {
            var httpClientHandler = new HttpClientHandler();

            //httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => {
            //    Debug.WriteLine(errors);

            //    foreach(var e in chain.ChainElements)
            //    {
            //        Debug.WriteLine($"CERT CHAIN: {e.Certificate.Subject} {e.Certificate.GetCertHashString()}");
            //    }

            //    return errors == System.Net.Security.SslPolicyErrors.None;
            //};

            return httpClientHandler;


        }

        public async Task<TokenResult> Authenticate(string username, string password)
        {
            UserMessage um = new UserMessage()
            {
                Username = username,
                Password = password
            };

            TokenResult token = null;
            string path = PrefixApiPath("/token");
            HttpContent httpContent = SerializeObject(um);

            HttpResponseMessage response = await Client.PostAsync(path, httpContent);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                token = JsonConvert.DeserializeObject<TokenResult>(data);
                userId = token.UserId.ToString();
                this.token = token.Token;
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);
            }
            return token;
        }

        public async Task<bool> Register(UserMessage user)
        {
            string path = PrefixApiPath("/register");

            HttpContent httpContent = SerializeObject(user);

            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task<DeviceMessage> GetDevice(string uniqueIdentifier)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{uniqueIdentifier}");
            HttpResponseMessage response = await client.GetAsync(path);
            DeviceMessage dm = null;
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                dm = JsonConvert.DeserializeObject<DeviceMessage>(data);
            }

            return dm;
        }

        public async Task<bool> RegisterDevice(string uniqueIdentifier,
                                               string model = "",
                                              string os = "",
                                               string manufacturer = "")
        {
            string path = PrefixApiPath($"/user/{userId}/device");
            DeviceMessage dm = new DeviceMessage()
            {
                UniqueIdentifier = uniqueIdentifier,
                Model = model,
                OperatingSystem = os,
                Manufacturer = manufacturer
            };

            HttpContent httpContent = SerializeObject(dm);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostPositions(string deviceId, List<PositionEntry> positions)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/positionentry");
            HttpContent httpContent = SerializeObject(positions);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostLogs(string deviceId, List<LogEntry> messages)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/logs");
            HttpContent httpContent = SerializeObject(messages);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostBatteryData(string deviceId, List<BatteryEntry> batteryEntries)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/battery");
            Debug.WriteLine(JsonConvert.SerializeObject(batteryEntries));
            HttpContent httpContent = SerializeObject(batteryEntries);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> IsDeviceUnauthorized(string deviceId)
        {
            string path = PrefixApiPath($"/validate_device?device={deviceId}");
            HttpResponseMessage response = await client.GetAsync(path);
            return response.StatusCode == System.Net.HttpStatusCode.Unauthorized;
        }
    }
}
