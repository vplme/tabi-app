using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private async Task<HttpContent> SerializeObjectAsync(object o)
        {
            return await Task.Run(() =>
            {
                return SerializeObject(o);
            });
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

            try
            {
                HttpResponseMessage response = await Client.PostAsync(path, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    token = JsonConvert.DeserializeObject<TokenResult>(data);
                    userId = token.UserId.ToString();
                    this.token = token.Token;
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("ApiClient error" + e);
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

        public async Task<DeviceMessage> GetDevice(int id)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{id}");
            DeviceMessage dm = null;

            try
            {
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    dm = JsonConvert.DeserializeObject<DeviceMessage>(data);
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return dm;
        }

        public async Task<DeviceCounts> GetDeviceCounts(int id)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{id}/counts");
            HttpResponseMessage response = await client.GetAsync(path);
            DeviceCounts dc = null;
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                dc = JsonConvert.DeserializeObject<DeviceCounts>(data);
            }

            return dc;
        }

        public async Task<DeviceMessage> RegisterDevice(string model = "",
                                                        string os = "", string osVersion = "",
                                               string manufacturer = "")
        {
            string path = PrefixApiPath($"/user/{userId}/device");
            DeviceMessage dm = new DeviceMessage()
            {
                Model = model,
                OperatingSystem = os,
                OperatingSystemVersion = osVersion,
                Manufacturer = manufacturer
            };

            HttpContent httpContent = SerializeObject(dm);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            string content = await response.Content.ReadAsStringAsync();

            DeviceMessage deviceResponse = JsonConvert.DeserializeObject<DeviceMessage>(content);

            return deviceResponse;
        }

        public async Task<bool> PostPositions(int deviceId, List<PositionEntry> positions)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/positionentry");
            HttpContent httpContent = await SerializeObjectAsync(positions);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostStops(int deviceId, List<Models.Stop> stops)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/stops");
            HttpContent httpContent = await SerializeObjectAsync(stops);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostUserStopMotives(int deviceId, List<Models.UserStopMotive> motives)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/stops/motives");
            HttpContent httpContent = await SerializeObjectAsync(motives);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostStopVisits(int deviceId, List<Models.StopVisit> stopVisits)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/stopvisits");
            HttpContent httpContent = await SerializeObjectAsync(stopVisits);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostLogs(int deviceId, List<LogEntry> messages)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/logs");
            HttpContent httpContent = await SerializeObjectAsync(messages);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostBatteryData(int deviceId, List<BatteryEntry> batteryEntries)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/battery");
            HttpContent httpContent = await SerializeObjectAsync(batteryEntries);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> PostTrackEntries(int deviceId, List<TabiApiClient.Models.TrackEntry> trackEntries)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/track");
            try
            {
                HttpContent httpContent = await SerializeObjectAsync(trackEntries);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> PostTransportationModes(int deviceId, IEnumerable<TabiApiClient.Models.TransportationMode> transportModes)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/mode");

            try
            {
                HttpContent httpContent = await SerializeObjectAsync(transportModes);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> PostSensorMeasurementSessions(int deviceId, List<SensorMeasurementSession> sensorMeasurementSessions)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/sensormeasurementsession");
            try
            {
                HttpContent httpContent = await SerializeObjectAsync(sensorMeasurementSessions);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }

        public async Task<bool> PostAccelerometerData(int deviceId, List<Accelerometer> accelerometerData)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/accelerometer");
            try
            {
                HttpContent httpContent = await SerializeObjectAsync(accelerometerData);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> PostGyroscopeData(int deviceId, List<Gyroscope> gyroscopeData)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/gyroscope");
            try
            {
                HttpContent httpContent = await SerializeObjectAsync(gyroscopeData);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }

        public async Task<bool> PostMagnetometerData(int deviceId, List<Magnetometer> magnetometerData)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/magnetometer");
            try
            {
                HttpContent httpContent = await SerializeObjectAsync(magnetometerData);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> PostLinearAccelerationData(int deviceId, List<LinearAcceleration> linearAcceleration)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/linearacceleration");
            try
            {
                HttpContent httpContent = await SerializeObjectAsync(linearAcceleration);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }

        public async Task<bool> PostGravityData(int deviceId, List<Gravity> gravity)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/gravity");
            try
            {
                HttpContent httpContent = await SerializeObjectAsync(gravity);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }

        public async Task<bool> PostQuaternionData(int deviceId, List<Quaternion> quaternionData)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/quaternion");
            try
            {
                HttpContent httpContent = await SerializeObjectAsync(quaternionData);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }

        public async Task<bool> PostOrientationData(int deviceId, List<Orientation> orientationData)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/orientation");
            try
            {
                HttpContent httpContent = await SerializeObjectAsync(orientationData);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }

        public async Task<bool> IsDeviceUnauthorized(int deviceId)
        {
            string path = PrefixApiPath($"/validate_device?device={deviceId}");
            HttpResponseMessage response = await client.GetAsync(path);
            return response.StatusCode == System.Net.HttpStatusCode.Unauthorized;
        }
    }
}
