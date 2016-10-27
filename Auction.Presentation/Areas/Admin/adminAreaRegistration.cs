using System.Web.Mvc;

namespace Auction.Presentation.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.Routes.IgnoreRoute("Admin/elmah.axd/{*pathInfo}");

            context.MapRoute(
            name: "Admin_def",
            url: "{lang}/Admin/",
            defaults: new { lang = "en-Us", action = "Index", controller = "Admin" });

            context.MapRoute(
            name: "Admin_elmah",
            url: "{lang}/Admin/elmah/{type}",
            defaults: new { lang = "en-Us", action = "Index", controller = "Elmah", type = UrlParameter.Optional });

            context.MapRoute(
            name: "Admin_elmah_detail",
            url: "{lang}/Admin/elmah/detail/{type}",
            defaults: new { lang = "en-Us", action = "Detail", controller = "Elmah", type = UrlParameter.Optional });

            context.MapRoute(
            name: "Admin",
            url: "{lang}/Admin/{controller}/{action}/{id}",
            defaults: new { lang = "en-Us", action = "Index", controller = "Home", id = UrlParameter.Optional });
        }
    }
}