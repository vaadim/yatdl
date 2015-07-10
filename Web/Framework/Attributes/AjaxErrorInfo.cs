using Newtonsoft.Json;

namespace YATDL
{
    public class AjaxErrorInfo
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("errorDetails")]
        public string ErrorDetails { get; set; }

        [JsonProperty("errorCode")]
        public int ErrorCode { get; set; }

        public AjaxErrorInfo()
        {
        }

        public AjaxErrorInfo(string error, int errorCode = 0) : this(error, null, errorCode)
        {
        }

        public AjaxErrorInfo(string error, string errorDetails, int errorCode = 0)
        {
            Error = error;
            ErrorDetails = errorDetails;
            ErrorCode = errorCode;
        }
    }
}