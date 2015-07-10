using System;
using System.Web.Mvc;
using Newtonsoft.Json;


namespace YATDL
{
    public class JsonNetResult : JsonResult
    {
        public JsonNetResult()
        {
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            StatusCode = 200;
        }

        public int StatusCode { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet && String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Метод GET не разрешен для отдачи JSON");


            var response = context.HttpContext.Response;
            response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data != null)
            {
                var serializer = new JsonSerializer();
                if (RecursionLimit.HasValue)
                    serializer.MaxDepth = RecursionLimit;

                serializer.Serialize(response.Output, Data);
            }

            if (StatusCode != 200)
                response.StatusCode = StatusCode;
        }
    }
}