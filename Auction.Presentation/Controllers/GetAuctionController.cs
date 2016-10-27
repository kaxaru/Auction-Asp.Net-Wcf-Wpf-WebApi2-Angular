using System.Web.Mvc;
using System.Web.Routing;
using Auction.Presentation.Localization;

namespace Auction.Presentation.Controllers
{
    public class GetAuctionController : Controller
    {
        public ActionResult Index(string id)
        {
            var routeValues = new RouteValueDictionary(Url.RequestContext.RouteData.Values);
            var langDefault = LocalizationService.GetDefaultLang().LocalizationId;

            if (!routeValues.ContainsKey("auction"))
            {
                routeValues.Add("auction", id);
            }
            else
            {
                routeValues["auction"] = id;
            }

            if (routeValues.ContainsKey("lang"))
            {
                langDefault = routeValues["lang"].ToString();
            }

            return RedirectToAction("Index", "Home", new { lang = langDefault, auction = id });
        }
    }
}