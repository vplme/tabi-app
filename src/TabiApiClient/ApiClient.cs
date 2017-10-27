using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TabiApiClient
{
    public class ApiClient
    {
        private static HttpClient client = new HttpClient();
        public HttpClient MockHttpClient { get; set; }
        public bool Mock { get; set; } = false;

        private string apiLocation;
        const string apiRoot = "/api/v1";

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

        public async Task<TokenResult> Authenticate(UserMessage user)
        {
            TokenResult token = null;
            string path = PrefixApiPath("/token");
            HttpContent httpContent = SerializeObject(user);

            HttpResponseMessage response = await Client.PostAsync(path, httpContent);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                Console.WriteLine(data);
                token = JsonConvert.DeserializeObject<TokenResult>(data);
            }
            return token;
        }

        public async Task Register(UserMessage user)
        {
            string path = PrefixApiPath("/register");

            HttpContent httpContent = SerializeObject(user);

            HttpResponseMessage response = await client.PostAsync(path, httpContent);
        }



    }
}
