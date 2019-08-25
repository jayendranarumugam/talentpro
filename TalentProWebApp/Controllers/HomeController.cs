using NLog;
using System.Web.Mvc;

namespace TalentProWebApp.Controllers
{
    public class HomeController : Controller
    {        

       [AllowAnonymous]
        public ActionResult Index()
        {           
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}