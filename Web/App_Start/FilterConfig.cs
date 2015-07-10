using System.Web.Mvc;

namespace YATDL
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleWithAjaxAndLogErrorAttribute());
        }
    }
}