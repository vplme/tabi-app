using System;
using Newtonsoft.Json;

namespace TabiApiClient
{
    public class TokenResult
    {
        public int UserId { get; set; }

        public string Token { get; set; }
    }
}
