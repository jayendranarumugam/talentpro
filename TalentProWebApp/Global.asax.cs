using Microsoft.ApplicationInsights.Extensibility;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TalentProWebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            TelemetryConfiguration.Active.InstrumentationKey = System.Web.Configuration.WebConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //DocumentDBRepository<ResumeDocModel>.Initialize();
        }


    }
}
