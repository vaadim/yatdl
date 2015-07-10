using System;
using System.Web.Mvc;


namespace YATDL
{
    public class HandleWithAjaxAndLogErrorAttribute : HandleAndLogErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            string accept = filterContext.HttpContext.Request.Headers["Accept"] ?? "";
            if (!filterContext.HttpContext.Request.IsAjaxRequest() && !accept.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                base.OnException(filterContext);
                return;
            }

            if (filterContext.IsChildAction || filterContext.ExceptionHandled)
                return;

            if (!filterContext.ExceptionHandled && this.IsNeedLog)
            {
                // Логируем исключение только если исключение помечено как необработанное
                var logger = DependencyResolver.Current.GetService<Logger>();
                logger.Error(filterContext.Exception.Message, filterContext.Exception);
            }

            Exception ex = filterContext.Exception;

            // Переопределяем результат в JSON
            filterContext.Result = new JsonNetResult
                                   {
                                       Data = new AjaxErrorInfo { Error = ex.Message, ErrorDetails = ex.StackTrace, ErrorCode = 0 },
                                       JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                   };

            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.ExceptionHandled = true;
        }
    }
}