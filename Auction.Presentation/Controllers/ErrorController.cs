using System.Web.Mvc;

namespace Auction.Presentation.Controllers
{
    public class ErrorsController : Controller
    {
        public ActionResult NotFound()
        {
            ViewBag.Path = Request.Url.PathAndQuery;
            return View();
        }

        public ActionResult Error401()
        {
            return View();
        }

        public ActionResult UserAuthorize()
        {
            return View();
        }
    }
}