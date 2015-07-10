using Newtonsoft.Json;

namespace YATDL.Models
{
    public class ValidationError
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}