using System.Web.Mvc;

namespace YATDL
{
    public abstract class YATDLViewPage<TModel> : WebViewPage<TModel>
    {
        /// <summary>
        /// Название текущего контроллера
        /// </summary>
        public string CurrentControllerName { get; private set; }

        /// <summary>
        /// Название текущего действия
        /// </summary>
        public string CurrentActionName { get; private set; }

        public override void InitHelpers()
        {
            base.InitHelpers();
            CurrentControllerName = ViewContext.RouteData.GetRequiredString("controller");
            CurrentActionName = ViewContext.RouteData.GetRequiredString("action");
        }

        
    }

    public abstract class YATDLViewPage : WebViewPage<dynamic>
    {
    }
}