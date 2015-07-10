using Newtonsoft.Json;

namespace YATDL
{
    public class GridTaskModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public GridTaskModel()
        {
        }


        public GridTaskModel(Task t)
        {
            Id = t.Id;
            Name = t.Name;
        }
    }
}