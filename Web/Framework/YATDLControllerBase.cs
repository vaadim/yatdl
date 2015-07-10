using System.Net;
using System.Text;
using System.Web.Mvc;

namespace YATDL
{
    public class YATDLControllerBase : Controller
    {
        public readonly Logger Logger;

        public YATDLControllerBase()
        {
            Logger = ServiceLocator.Current.GetInstance<Logger>();
        }

        #region JSON

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return base.Json(data, contentType, contentEncoding, JsonRequestBehavior.AllowGet);
        }

        //protected internal new JsonResult Json(object data)
        //{
        //    return Json(data, null /* contentType */, null /* contentEncoding */, JsonRequestBehavior.AllowGet);
        //}

        //protected internal new JsonResult Json(object data, string contentType)
        //{
        //    return Json(data, contentType, null /* contentEncoding */, JsonRequestBehavior.AllowGet);
        //}

        //protected override JsonResult Json(object data, string contentType, Encoding contentEncoding)
        //{
        //    return Json(data, contentType, contentEncoding, JsonRequestBehavior.AllowGet);
        //}

        //protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        //{
        //    return new JsonNetResult
        //    {
        //        Data = data,
        //        ContentType = contentType,
        //        ContentEncoding = contentEncoding,
        //        JsonRequestBehavior = behavior
        //    };
        //}


        protected JsonResult JsonNet(object data, string contentType = null, Encoding contentEncoding = null, JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }




        protected internal JsonNetResult JsonError(AjaxErrorInfo data, int statusCode)
        {
            return new JsonNetResult
            {
                Data = data,
                StatusCode = statusCode
            };
        }

        protected internal JsonNetResult JsonError(AjaxErrorInfo data, HttpStatusCode statusCode)
        {
            return JsonError(data, (int)statusCode);
        }

        /// <summary>
        /// Ошибка в ajax. Передается как {error: errorMessage}
        /// </summary>
        /// <param name="error"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        protected internal JsonNetResult JsonError(string error, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            return JsonError(new AjaxErrorInfo(error), statusCode);
        }

        #endregion JSON

    }
}
