using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TabiApiClient.Http;
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
        private bool gzipEnabled;
        private string userId;

        public ApiClient(string clientIdentifier, string clientKey, string apiLocation = "https://tabi.0x2a.site", bool gzip = false)
        {
            this.apiLocation = apiLocation;
            HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("X-Tabi-Client-Identifier", clientIdentifier);
            client.DefaultRequestHeaders.Add("X-Tabi-Client-Key", clientKey);
            client.Timeout = TimeSpan.FromMinutes(5);
            gzipEnabled = gzip;
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

        private HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                if (gzipEnabled)
                {
                    httpContent = new GzipJsonContent(content);
                }
                else
                {
                    httpContent = new JsonContent(content);
                }
            }

            return httpContent;
        }

        public async Task<TokenResult> Authenticate(string username, string password)
        {
            UserMessage um = new UserMessage()
            {
                Username = username,
                Password = password
            };

            TokenResult tokenResult = null;
            string path = PrefixApiPath("/token");
            HttpContent httpContent = CreateHttpContent(um);

            try
            {
                HttpResponseMessage response = await Client.PostAsync(path, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    tokenResult = JsonConvert.DeserializeObject<TokenResult>(data);
                    userId = tokenResult.UserId.ToString();
                    this.token = tokenResult.Token;
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("ApiClient error" + e);
            }

            return tokenResult;
        }

        public async Task<bool> Register(UserMessage user)
        {
            string path = PrefixApiPath("/register");

            HttpContent httpContent = CreateHttpContent(user);

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

        public async Task<string> GetDeviceConfig(int id)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{id}/config");
            string data = null;

            try
            {
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    data = await response.Content.ReadAsStringAsync();
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return data;
        }

        public async Task<bool> Ping(int timeout = 10)
        {
            bool result = false;
            string path = PrefixApiPath($"/ping");
            Ping ping = null;
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(timeout * 1000);
            try
            {
                HttpResponseMessage response = await client.GetAsync(path, cts.Token);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    ping = JsonConvert.DeserializeObject<Ping>(data);
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            if (ping != null && ping.Available)
            {
                result = true;
            }

            return result;
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

            HttpContent httpContent = CreateHttpContent(dm);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            string content = await response.Content.ReadAsStringAsync();

            DeviceMessage deviceResponse = JsonConvert.DeserializeObject<DeviceMessage>(content);

            return deviceResponse;
        }


        public async Task<bool> SetAttribute(int deviceId, string attributeKey, string value)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/attribute/{attributeKey}");

            HttpContent httpContent = CreateHttpContent(value);
            HttpResponseMessage response = await client.PutAsync(path, httpContent);
            string content = await response.Content.ReadAsStringAsync();

            return response.IsSuccessStatusCode;
        }

        public async Task<string> GetAttribute(int deviceId, string attributeKey)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/attribute/{attributeKey}");
            HttpResponseMessage response = await client.GetAsync(path);
            string content = await response.Content.ReadAsStringAsync();
            string valueResponse = JsonConvert.DeserializeObject<string>(content);

            return valueResponse;
        }

        public async Task<bool> PutAttribute(int deviceId, string key, string value)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/attribute");

            Models.Attribute attribute = new Models.Attribute()
            {
                Key = key,
                Value = value
            };

            HttpContent httpContent = CreateHttpContent(attribute);
            HttpResponseMessage response = await client.PutAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostPositions(int deviceId, IEnumerable<PositionEntry> positions)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/positionentry");
            HttpContent httpContent = CreateHttpContent(positions);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostStops(int deviceId, IEnumerable<Stop> stops)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/stops");
            HttpContent httpContent = CreateHttpContent(stops);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostQuestions(int deviceId, IEnumerable<Question> questions)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/question");
            HttpContent httpContent = CreateHttpContent(questions);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostUserStopMotives(int deviceId, IEnumerable<UserStopMotive> motives)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/stops/motives");
            HttpContent httpContent = CreateHttpContent(motives);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostStopVisits(int deviceId, IEnumerable<StopVisit> stopVisits)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/stopvisits");
            HttpContent httpContent = CreateHttpContent(stopVisits);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostLogs(int deviceId, IEnumerable<Log> logs)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/logs");
            HttpContent httpContent = CreateHttpContent(logs);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostBatteryData(int deviceId, IEnumerable<BatteryInfo> batteryEntries)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/battery");
            HttpContent httpContent = CreateHttpContent(batteryEntries);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> PostTrackEntries(int deviceId, IEnumerable<TrackEntry> trackEntries)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/track");
            try
            {
                HttpContent httpContent = CreateHttpContent(trackEntries);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> PostTrackMotives(int deviceId, IEnumerable<TrackMotive> motives)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/track/motives");
            HttpContent httpContent = CreateHttpContent(motives);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostTransportationModes(int deviceId, IEnumerable<TransportationMode> transportModes)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/mode");

            try
            {
                HttpContent httpContent = CreateHttpContent(transportModes);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> PostSensorMeasurementSessions(int deviceId, IEnumerable<SensorMeasurementSession> sensorMeasurementSessions)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/sensormeasurementsession");
            try
            {
                HttpContent httpContent = CreateHttpContent(sensorMeasurementSessions);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }

        public async Task<bool> PostAccelerometerData(int deviceId, IEnumerable<MotionSensor> accelerometerData)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/accelerometer");
            try
            {
                HttpContent httpContent = CreateHttpContent(accelerometerData);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> PostGyroscopeData(int deviceId, IEnumerable<MotionSensor> gyroscopeData)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/gyroscope");
            try
            {
                HttpContent httpContent = CreateHttpContent(gyroscopeData);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }

        public async Task<bool> PostMagnetometerData(int deviceId, IEnumerable<MotionSensor> magnetometerData)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/magnetometer");
            try
            {
                HttpContent httpContent = CreateHttpContent(magnetometerData);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> PostLinearAccelerationData(int deviceId, IEnumerable<MotionSensor> linearAcceleration)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/linearacceleration");
            try
            {
                HttpContent httpContent = CreateHttpContent(linearAcceleration);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }

        public async Task<bool> PostGravityData(int deviceId, IEnumerable<MotionSensor> gravity)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/gravity");
            try
            {
                HttpContent httpContent = CreateHttpContent(gravity);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }

        public async Task<bool> PostQuaternionData(int deviceId, IEnumerable<Quaternion> quaternionData)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/quaternion");
            try
            {
                HttpContent httpContent = CreateHttpContent(quaternionData);
                HttpResponseMessage response = await client.PostAsync(path, httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }

        public async Task<bool> PostOrientationData(int deviceId, IEnumerable<MotionSensor> orientationData)
        {
            string path = PrefixApiPath($"/user/{userId}/device/{deviceId}/orientation");
            try
            {
                HttpContent httpContent = CreateHttpContent(orientationData);
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
