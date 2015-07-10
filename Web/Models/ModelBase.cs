using System.Web.Mvc;
using Newtonsoft.Json;

namespace YATDL
{
    public class ModelBase
    {
        [JsonProperty("id")]
        [HiddenInput]
        public int Id { get; set; }

        public bool IsNew()
        {
            return Id == 0;
        }
    }
}