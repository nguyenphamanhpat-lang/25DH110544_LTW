using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyStoreMVC5
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            App_Start.RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
