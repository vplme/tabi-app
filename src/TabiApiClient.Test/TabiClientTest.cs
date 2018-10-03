using System.Net.Http;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using Xunit;

namespace TabiApiClient.Test
{
    public class TabiClientTest
    {
        [Fact]
        public async Task TestAuthenticateAsync()
        {
            ApiClient client = new ApiClient("ApiClient-Test", "TEST", "http://localhost:8000")
            {
                MockHttpClient = SetupMockHttpClient(),
                Mock = true
            };

            TokenResult token = await client.Authenticate("user", "password");

            Assert.NotNull(token);
            Assert.Equal("test_token", token.Token);
            Assert.Equal(1, token.UserId);
        }

        private HttpClient SetupMockHttpClient()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost:8000/api/v1/token")
                    .Respond("application/json", "{'UserId': 1, 'Token': 'test_token'}");

            return mockHttp.ToHttpClient();
        }

    }
}
