using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Auction.Presentation
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("robots.txt");
            routes.IgnoreRoute("favicon.ico");

            /*routes.MapRoute(
                name: "lang",
                url: "{lang}/{controller}/{action}/{id}",
                defaults: new { lang = "en-Us", controller = "Home", action = "Index", id = UrlParameter.Optional });
            */

            routes.MapRoute(
                    name: "Auction",
                    url: "{lang}/{auction}/{controller}/{action}/{id}",
                    defaults: new { lang = "en-Us", auction = "Auction1", controller = "Home", action = "Index", id = UrlParameter.Optional });
            }
    }
}
