using Newtonsoft.Json;

namespace dominfo
{
    public class TokenResponse
    {
        [JsonProperty("response")]
        public Token Response { get; set; }
    }
    
    public class Token
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }
        [JsonProperty("time")]
        public string Time { get; set; }
    }
}