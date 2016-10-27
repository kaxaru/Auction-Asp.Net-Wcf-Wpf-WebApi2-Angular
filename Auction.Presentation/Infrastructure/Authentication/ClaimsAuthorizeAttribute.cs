using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Auction.Presentation.Infrastructure.СustomSettings;

namespace Auction.Presentation.Infrastructure.Authentication
{
    public class ClaimsAuthorizeAttribute : AuthorizeAttribute
    {
        private static AuctionHousesSection config = ConfigurationManager.GetSection("AuctionHouses") as AuctionHousesSection;
        private string[] allowedRoles = new string[] { };     

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.Result is HttpUnauthorizedResult)
            {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                            { "area",  string.Empty },
                            { "controller", "Errors" },
                            { "action", "UserAuthorize" },
                            { "ReturnUrl", filterContext.HttpContext.Request.RawUrl }
                        });
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            ClaimsIdentity claimsIdentity;
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            claimsIdentity = httpContext.User.Identity as ClaimsIdentity;
            var roleClaims = claimsIdentity.FindFirst(ClaimTypes.Role);
            List<string> roles = new List<string>();
            if (roleClaims == null)
            {
                foreach (AuctionHouseElement auction in config.AuctionHouses)
                {
                    var currClaim = claimsIdentity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/" + auction.Name);
                    if (currClaim != null)
                    {
                        roles.Add(currClaim.Value);
                    }
                }
            }
            else
            {
                roles.Add(roleClaims.Value);
            }

            return Role(roles);
        }

        private bool Role(List<string> roles)
        {
            if (!string.IsNullOrEmpty(Roles))
            {
                allowedRoles = Roles.Split(new char[] { ',' });
                for (int i = 0; i < allowedRoles.Length; i++)
                {
                    allowedRoles[i] = allowedRoles[i].Trim().ToLower();
                }
            }

            if (allowedRoles.Length > 0)
            {
                for (int i = 0; i < allowedRoles.Length; i++)
                {
                    foreach (string role in roles)
                    {
                        if (role.ToLower() == allowedRoles[i].ToLower())
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                return false;
            }

            return false;
        }
    }
}