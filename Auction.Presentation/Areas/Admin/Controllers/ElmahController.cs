using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Auction.Presentation.App_Start;
using Auction.Presentation.Infrastructure.Authentication;

namespace Auction.Presentation.Areas.Admin.Controllers
{
    [ClaimsAuthorize(Roles = "admin")]
    public class ElmahController : Controller
    {
        // GET: Elmah
        public ActionResult Index(string type)
        {
            return new ElmahResult(type);
        }

        public ActionResult Detail(string type)
        {
            return new ElmahResult(type);
        }
    }
}