using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace TalentProWebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(name: "Resume", url: "{controller}/{action}",
                     defaults:new
                     {
                         controller = "Home",
                         action = "Index"                         
                     });
        }
    }
}
