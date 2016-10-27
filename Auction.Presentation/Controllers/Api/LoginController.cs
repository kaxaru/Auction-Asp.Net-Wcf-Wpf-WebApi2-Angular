using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Auction.Presentation.Infrastructure.Authentication;
using Auction.Presentation.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Omu.ValueInjecter;

namespace Auction.Presentation.Controllers.Api
{
    public class LoginController : ApiController
    {
        private readonly CustomUserManager _customUserManager;

        public LoginController(CustomUserManager userManager)
        {
            _customUserManager = userManager;
        }

        protected IAuthenticationManager AuthenticationManager => HttpContext.Current.GetOwinContext().Authentication;

        [HttpPost]
        [Route("api/login/logIn")]
        public async Task<HttpResponseMessage> Login(LoginViewModel login)
        {
            var user = Mapper.Map<UserViewModel>(login);
            user = await _customUserManager.FindAsync(login.UserName, login.Password);
            if (user == null)
            {
                BadRequest();
            }

            var identity = await _customUserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            identity.AddClaim(new Claim(ClaimService.ClaimsType["timezone"], user.TimezoneId));
            ////await _customUserManager.AddClaimAsync(user.Id, new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/timezone", user.TimezoneId));
            AuthenticationManager.SignIn(identity);           
            user.Password = string.Empty;
            user.Picture = string.Empty;
            var serializeUser = JsonConvert.SerializeObject(user);
            var cookie = new CookieHeaderValue("User", serializeUser);
            cookie.Expires = DateTimeOffset.Now.AddDays(1);
            cookie.Domain = Request.RequestUri.Host;
            cookie.Path = "/";
            ////cookie.HttpOnly = true;
            var response = new HttpResponseMessage();
            response.Headers.AddCookies(new CookieHeaderValue[] { cookie });
            return response;
        }

        [HttpPost]
        [Route("api/login/logOut")]
        public HttpResponseMessage LogOut()
        {
            var response = new HttpResponseMessage();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            CookieHeaderValue cookie = Request.Headers.GetCookies("User").FirstOrDefault();
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1d);
                cookie.Domain = Request.RequestUri.Host;
                cookie.Path = "/";
                response.Headers.AddCookies(new CookieHeaderValue[] { cookie });
            }

            return response;
        }
    }
}
