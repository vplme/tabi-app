using System;
using Newtonsoft.Json;

namespace TabiApiClient
{
    public class TokenResult
    {
        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty]
        public string Token { get; set; }
    }
}
