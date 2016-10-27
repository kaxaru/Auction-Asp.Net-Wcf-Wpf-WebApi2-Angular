using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Auction.Presentation.Infrastructure.Authentication;
using Auction.Presentation.Localization;
using Auction.Presentation.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Omu.ValueInjecter;

namespace Auction.Presentation.Controllers
{
    public class LoginController : Controller
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly CustomUserManager _customUserManager;
                     
        public LoginController(CustomUserManager userManager)
        {
            _customUserManager = userManager;
        }       

        protected IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        public ActionResult Login()
        {
            return PartialView(new LoginViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> LoginToPost(LoginViewModel login)
        {
            var user = Mapper.Map<UserViewModel>(login);
                user = await _customUserManager.FindAsync(login.UserName, login.Password);
           if (user == null)
            {
                HttpCookie userNotFound = new HttpCookie("userNotFound");
                userNotFound["date"] = DateTime.Now.ToShortTimeString();
                userNotFound.Expires = DateTime.Now.AddHours(1);
                Response.Cookies.Add(userNotFound);
                return RedirectToAction("Index", "Home");
            }        
                
            var identity = await _customUserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);           
            identity.AddClaim(new Claim(ClaimService.ClaimsType["timezone"], user.TimezoneId));
            ////await _customUserManager.AddClaimAsync(user.Id, new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/timezone", user.TimezoneId));
            AuthenticationManager.SignIn(identity);
            _logger.Info(string.Format("user logged with id - {0}, username -{1}, timezone - {2}", user.Id, user.UserName, user.TimezoneId));
            return RedirectToAction("Index", "Home", new { lang = user.Locale, auction = "Auction1" });
        }

        [Authorize]
        public ActionResult Logout()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var id = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var username = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Name);
            var timezoneId = claimsIdentity.FindFirst(ClaimService.ClaimsType["timezone"]);
            _logger.Info(string.Format("user logged with id - {0}, username -{1}, timezone - {2}", id.Value, username.Value, timezoneId.Value));
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home", new { lang = LocalizationService.GetDefaultLang().LocalizationId });
        }
    }
}