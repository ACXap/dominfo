using Newtonsoft.Json;

namespace dominfo
{
    public class DataResponse
    {
        [JsonProperty("response")]
        public DataHtml DataHtml { get; set; }
    }

    public class DataHtml
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}