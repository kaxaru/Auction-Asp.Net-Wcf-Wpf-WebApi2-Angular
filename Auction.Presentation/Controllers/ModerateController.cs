using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Auction.BussinessLogic.Models;
using Auction.BussinessLogic.Services;
using Auction.Presentation.Infrastructure;
using Auction.Presentation.Infrastructure.Authentication;
using Auction.Presentation.Infrastructure.СustomSettings;
using Auction.Presentation.Models;
using Newtonsoft.Json;
using Omu.ValueInjecter;

namespace Auction.Presentation.Controllers
{
    [ClaimsAuthorize(Roles = "moderator, admin")]
    public class ModerateController : Controller
    {
        private static AuctionHousesSection config = ConfigurationManager.GetSection("AuctionHouses") as AuctionHousesSection;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IProductService _productService;
        private readonly DataAccess.Repositories.IAuctionProvider _aucProvider;
        private readonly CustomUserManager _customUserManager;

        public ModerateController(CustomUserManager customUserManager, DataAccess.Repositories.IAuctionProvider aucProvider, IProductService productService)
        {
            _aucProvider = aucProvider;
            _productService = productService;
            _customUserManager = customUserManager;
        }

        private string ClaimId
        {
            get
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var claimId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                return claimId;
            }
        }

        public async Task<ActionResult> Index()
        {
            var listPermissions = (await _customUserManager.GetAllPermissionsAsync()).Where(p => p.UserId == Guid.Parse(ClaimId));
            var dictionaryProduct = new Dictionary<string, IList<ProductViewModel>>();
            foreach (var permission in listPermissions)
            {
                if ((int)permission.Role == (int)Models.Role.Admin)
                {
                    foreach (AuctionHouseElement auction in config.AuctionHouses)
                    {
                        Auctions.SetAuction(auction);
                        var draftProducts = await _productService.ShowProductsForModerationAsync();
                        dictionaryProduct.Add(auction.Name, draftProducts.Select(pr => Mapper.Map<ProductViewModel>(pr)).ToList());
                    }
                }

                if ((int)permission.Role == (int)Models.Role.Moderator)
                {
                    var currAuction = config.AuctionHouses.Search(permission.AuctionId);
                    Auctions.SetAuction(currAuction);

                    var draftProducts = await _productService.ShowProductsForModerationAsync(permission.CategoriesId);
                    if (draftProducts.Count() > 0)
                    {
                        dictionaryProduct.Add(permission.AuctionId, draftProducts.Select(pr => Mapper.Map<ProductViewModel>(pr)).ToList());
                    }
                }
            }
        
            return View(dictionaryProduct);
        }

        [HttpPost]
        public async Task<JsonResult> Index(string products)
        {
            dynamic listProducts = new ExpandoObject();
            if (Request.IsAjaxRequest())
            {
                try
                {
                    listProducts = JsonConvert.DeserializeObject(products);
                    foreach (var product in (IEnumerable<dynamic>)listProducts)
                    {
                        var auction = product.auction.ToString();
                        var currAuction = config.AuctionHouses.Search(auction);
                        Auctions.SetAuction(currAuction);
                        ProductDTO currProduct = await _productService.GetProductAsync(Guid.Parse(product.id.ToString()));
                        currProduct.State = (State)Enum.Parse(typeof(State), product.state.ToString());
                        currProduct.StartDate = DateTime.Now;
                        await _productService.EditProductAsync(currProduct);
                        _logger.Info(string.Format("moderator change state on product  - {0}, auction -{1}", currProduct.Id, auction));
                    }
                    
                    return Json(JsonConvert.SerializeObject(Url.Action("Index", "Home", new { lang = "en-Us", auction = "Auction1" })));
                }
                catch (Exception)
                {
                }
            }

            return Json(JsonConvert.SerializeObject(Url.Action("Index", "Moderate", new { lang = "en-Us", auction = "Auction1" })));
        }
    }
}