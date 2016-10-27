using System.Web.Mvc;

namespace Auction.Presentation.Areas.Spa.Controllers
{
    public class SpaController : Controller
    {
        // GET: Spa/Spa
        public ActionResult Index()
        {
            return View();
        }
    }
}