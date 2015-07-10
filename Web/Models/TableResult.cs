using Newtonsoft.Json;

namespace YATDL
{
    public class TableResult
    {
        [JsonProperty("result")]
        public object Result { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("skip")]
        public int? Skip { get; set; }

        [JsonProperty("take")]
        public int? Take { get; set; }

        public TableResult()
        {
        }


        public TableResult(object result, int totalCount, int? skip = null, int? take = null)
        {
            this.Result = result;
            this.TotalCount = totalCount;
            this.Skip = skip;
            this.Take = take;
        }
    }
}