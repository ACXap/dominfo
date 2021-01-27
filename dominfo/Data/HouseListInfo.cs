using dominfo.Helpers;
using Newtonsoft.Json;

namespace dominfo
{
    public class HouseListInfo
    {
        [JsonProperty("company_id")]
        public int CompanyId { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("offset")]
        public int Offset { get; set; }
        [JsonProperty("order_by")]
        public string OrderBy { get; set; }
        [JsonProperty("sort")]
        public string Sort { get; set; }
        [JsonProperty("all")]
        public int All { get; set; }
        [JsonProperty("all_const")]
        public int AllConst { get; set; }

        public string GetLoadAll()
        {
            Count = 500000;
            Offset = 0;
            OrderBy = "year";
            Sort = "desc";
            All = 500000;
            AllConst = 500000;

            string content = JsonConvert.SerializeObject(this);
            return Base.Base64Encode(content);
        }
    }

}