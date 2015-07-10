using System;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace YATDL
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DependencyRegistrator.RegisterDependencies();

            using (var m = new YATDLContext())
            {
                if (!m.Database.Exists())
                {
                    m.Database.Initialize(false);
                    Roles.CreateRole("Administrator");
                }

                var adminUsers = Membership.FindUsersByName("admin");
                if (adminUsers == null || adminUsers.Count == 0)
                {
                    Membership.CreateUser("admin", "admin", "admin@todo.ru");
                    Roles.AddUserToRole("admin", "Administrator");
                }
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //TODO: Сделать зависимым от настроек пользователя
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru");
        }
    }
}