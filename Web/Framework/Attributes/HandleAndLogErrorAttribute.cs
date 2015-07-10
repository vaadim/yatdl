using System;
using System.Web;
using System.Web.Mvc;


namespace YATDL
{
    /// <summary>
    /// Атрибут, используемый для обработки и логирования исключений
    /// </summary>
    public class HandleAndLogErrorAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// Требуется ли логирование исключения (по умолчанию true)
        /// </summary>
        public bool IsNeedLog { get; set; }

        public HandleAndLogErrorAttribute()
        {
            IsNeedLog = true;
        }

        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled && this.IsNeedLog)
            {
                // Логируем исключение только если исключение помечено как необработанное
                var logger = DependencyResolver.Current.GetService<Logger>();
                logger.Error(filterContext.Exception.Message, filterContext.Exception);
            }


            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                base.OnException(filterContext);
            }
            else
            {
                if (filterContext.IsChildAction)
                    return;

                if (filterContext.ExceptionHandled)
                    return;

                Exception exception = filterContext.Exception;

                // If this is not an HTTP 500 (for example, if somebody throws an HTTP 404 from an action method),
                // ignore it.
                if (new HttpException(null, exception).GetHttpCode() != 500)
                    return;

                if (!ExceptionType.IsInstanceOfType(exception))
                    return;

                filterContext.Result = new JsonResult
                                       {
                                           Data = new {error = exception.Message, errorDetails = exception.StackTrace},
                                           JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                       };

                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = 500;
            }
        }
    }
}