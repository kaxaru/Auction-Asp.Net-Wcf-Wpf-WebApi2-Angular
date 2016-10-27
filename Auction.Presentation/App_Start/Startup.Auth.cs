using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Auction.Presentation.Infrastructure.Authentication;
using Auction.Presentation.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace Auction.Presentation
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext<CustomUserManager>(CustomUserManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<UserManager<UserViewModel, string>, UserViewModel>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie))
                }
            });
        }
    }
}