using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Web.Mvc;
using System.Web;
using Microsoft.Owin.Security.Cookies;
using System.Security.Claims;
using System.Threading;
using System.Linq;

namespace TalentProWebApp.Controllers
{
    public class AccountController : Controller
    {
        // Sends an OpenIDConnect Sign-In Request.  
        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        
        /// <summary>
        /// Send an OpenID Connect sign-out request.
        /// </summary>
        public void SignOut()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(
                OpenIdConnectAuthenticationDefaults.AuthenticationType,
                CookieAuthenticationDefaults.AuthenticationType);
        }

        [Authorize]
        public static string GetUserName()
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            string name = identity.Claims.Where(c => c.Type == "name").Select(c => c.Value).SingleOrDefault();
            return name;
        }
    }
}