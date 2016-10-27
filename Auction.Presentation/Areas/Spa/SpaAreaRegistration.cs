using System.Web.Mvc;

namespace Auction.Presentation.Areas.Spa
{
    public class SpaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Spa";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Spa_default",
                "{lang}/Spa/",
                new { lang = "en-Us", controller = "Spa", action = "Index" });
        }
    }
}