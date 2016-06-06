using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace NtTracker
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var defaultCulture = ConfigurationManager.AppSettings["DefaultCulture"];

            routes.MapRoute(
                name: "Default",
                url: "{culture}/{controller}/{action}/{id}",
                defaults: new { culture = defaultCulture, controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
