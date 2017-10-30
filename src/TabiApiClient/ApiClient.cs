using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tabi.DataObjects;
using TabiApiClient.Messages;

namespace TabiApiClient
{
    public class ApiClient
    {
        private static HttpClient client = new HttpClient();
        public HttpClient MockHttpClient { get; set; }
        public bool Mock { get; set; } = false;

        private string apiLocation;
        const string apiRoot = "/api/v1";
        private string token;
        private string userId;

        public ApiClient(string apiLocation = "http://localhost:8000") => this.apiLocation = apiLocation;

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
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.token}");
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

        public async Task<DeviceMessage>GetDevice(string uniqueIdentifier)
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
            Debug.WriteLine(JsonConvert.SerializeObject(dm));
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            if(response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task SendPositions(string deviceId, List<PositionEntry> positions)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/positionentry");
            HttpContent httpContent = SerializeObject(positions);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
        }

    }
}
