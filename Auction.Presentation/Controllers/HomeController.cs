using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Auction.BussinessLogic.Infrastructure.Extendion;
using Auction.BussinessLogic.Services;
using Auction.Presentation.Infrastructure;
using Auction.Presentation.Infrastructure.Authentication;
using Auction.Presentation.Infrastructure.СustomSettings;
using Auction.Presentation.Models;
using Omu.ValueInjecter;

namespace Auction.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private static AuctionHousesSection config = ConfigurationManager.GetSection("AuctionHouses") as AuctionHousesSection;
        private readonly DataAccess.Repositories.IAuctionProvider _auctionProvider;
        private readonly ICategoriesService _categoryService;
        private readonly IProductService _productService;
        private readonly IBidService _bidService;
        private readonly CustomUserManager _customUserManager;

        public HomeController(DataAccess.Repositories.IAuctionProvider auctionProvider,
                                ICategoriesService categoryService, 
                                IProductService productService, 
                                IBidService bidService, 
                                CustomUserManager customUserManager)
        {
            _auctionProvider = auctionProvider;
            _productService = productService;
            _categoryService = categoryService;
            _bidService = bidService;
            _customUserManager = customUserManager;
        }

        public int BidStep
        {
            get
            {
                int val;
                int.TryParse(ConfigurationManager.AppSettings["bidStep"], out val);
                return val;
            }
        }

        public async Task<ActionResult> Index(string lang = "en-Us", string auction = "Auction1", int page = 1)
        {
                var auctionModel = config.AuctionHouses.Search(auction);
                if (auctionModel != null)
                {
                    Auctions.SetAuction(auctionModel);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var listProducts = await _productService.ShowAwalaibleProductsAsync();
                if (listProducts.IsNullOrEmpty())
                {
                    return View("~/Views/Home/EmptyModel.cshtml");
                }

                PageInfo pageInfo = new PageInfo { PageNumber = page, TotalItems = listProducts.Count() };

                var listLastBid = await _bidService.ShowLastBidForListProductAsync(listProducts);

                var listViewProducts = listProducts.Select(pr => new ProductClientViewModel
                {
                    Id = pr.Id,
                    CategoryID = pr.CategoryID,
                    Description = pr.Description,
                    Duration = pr.Duration.TotalSeconds,
                    Name = pr.Name,
                    Picture = pr.Picture,
                    StartDate = pr.StartDate,
                    StartPrice = pr.StartPrice
                });

                IEnumerable<ProductClientViewModel> productPerPages = listViewProducts.Skip((page - 1) * pageInfo.PageSize).Take(pageInfo.PageSize);

                var listBidViewModel = listLastBid.Select(b => Mapper.Map<BidViewModel>(b));
                var userResult = await _customUserManager.GetAllUsersAsync();
                var userVM = userResult.Select(u => Mapper.Map<ProfileViewModel>(u));
                ProductsWithBidViewModel categoryWithBids = new ProductsWithBidViewModel()
                {
                    Products = productPerPages,
                    PageInfo = pageInfo,
                    Bids = listBidViewModel,
                    Users = userVM
                };
                ViewBag.BidStep = BidStep;
                ViewBag.Auction = auctionModel.Name;
                return View("~/Views/Home/Index.cshtml", categoryWithBids);                      
        }

         public async Task<ActionResult> Category(Guid? id, string lang = "en-Us", string auction = "Auction1")
         {
            var auctionModel = config.AuctionHouses.Search(auction);
            if (auctionModel != null)
            {
                Auctions.SetAuction(auctionModel);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (id == null)
             {
                 return null;
             }

             var products = await _productService.ShowProductsFromCategoryAsync(id.GetValueOrDefault());
             if (products == null || products.IsNullOrEmpty())
             {
                 return View("~/Views/Home/EmptyModel.cshtml");
             }

             var listLastBid = await _bidService.ShowLastBidForListProductAsync(products);

             var listViewProducts = products.Select(pr => new ProductClientViewModel
             {
                                                                                         Id = pr.Id,
                                                                                         CategoryID = pr.CategoryID,
                                                                                         Description = pr.Description,
                                                                                         Duration = pr.Duration.TotalSeconds,
                                                                                         Name = pr.Name,
                                                                                         Picture = pr.Picture,
                                                                                         StartDate = pr.StartDate
                                                                                     });
             var listBidViewModel = listLastBid.Select(b => Mapper.Map<BidViewModel>(b));
             var userResult = await _customUserManager.GetAllUsersAsync();
             var userVM = userResult.Select(u => Mapper.Map<ProfileViewModel>(u));
             ProductsWithBidViewModel categoryWithBids = new ProductsWithBidViewModel()       
             {
                 Products = listViewProducts,
                 Bids = listBidViewModel,
                 Users = userVM
             };
             ViewBag.BidStep = BidStep;
            ViewBag.Auction = auctionModel.Name;

            return View("~/Views/Home/Index.cshtml", categoryWithBids);
         }
    }
}