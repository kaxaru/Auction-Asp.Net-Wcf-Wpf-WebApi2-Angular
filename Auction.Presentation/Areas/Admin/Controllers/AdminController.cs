using System.Web.Mvc;
using Auction.Presentation.Infrastructure.Authentication;

namespace Auction.Presentation.Areas.Admin.Controllers
{
    [ClaimsAuthorize(Roles = "admin")]
    public class AdminController : Controller
    {        
        public ActionResult Index()
        {
            return View();
        }
    }
}