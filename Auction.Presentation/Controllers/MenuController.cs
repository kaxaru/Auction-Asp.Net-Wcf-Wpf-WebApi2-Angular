using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Auction.BussinessLogic.Services;
using Auction.Presentation.Infrastructure.СustomSettings;
using Auction.Presentation.Models;
using Omu.ValueInjecter;

namespace Auction.Presentation.Controllers
{
    public class MenuController : Controller
    {
        private static AuctionHousesSection config = ConfigurationManager.GetSection("AuctionHouses") as AuctionHousesSection;
        private readonly ICategoriesService _categoryService;
        private readonly DataAccess.Repositories.IAuctionProvider _auctionProvider;

        public MenuController(DataAccess.Repositories.IAuctionProvider auctionProvider, ICategoriesService categoryService)
        {
            _auctionProvider = auctionProvider;
            _categoryService = categoryService;
        }

        // GET: Menu
        [ChildActionOnly]
        public ActionResult Categories()
        {
            object auctionName;
            AuctionHouseElement auction = null;
            List<AuctionHouseElement> auctions = new List<AuctionHouseElement>();

            if (Url.RequestContext.RouteData.Values.TryGetValue("auction", out auctionName))
            {
                 auction = config.AuctionHouses.Search((string)auctionName);
                if (auction != null)
                {
                    Infrastructure.Auctions.SetAuction(auction);
                }
            }

            if (auction == null)
            {              
                foreach (AuctionHouseElement auctionEl in config.AuctionHouses)
                {
                    auctions.Add(new AuctionHouseElement() { Name = auctionEl.Name, Location = auctionEl.Location, Type = auctionEl.Type });
                }

                Infrastructure.Auctions.SetAuction(auctions.First());
            }

            IEnumerable<CategoryViewModel> categoriesVM = null;

            try
            {
                var categories = Task.Run(async () => 
                {
                    return await _categoryService.ShowAwalaibleCategoriesAsync();
                }).Result;

                categoriesVM = categories.Select(cat => Mapper.Map<CategoryViewModel>(cat));
            }
            catch (AggregateException)
            {
                ////log
            }
            
            return PartialView("~/Views/Shared/_Category.cshtml", categoriesVM);
        }

        public ActionResult Auctions()
        {
            List<Areas.Admin.Models.AuctionViewModel> auctions = new List<Areas.Admin.Models.AuctionViewModel>();
            foreach (AuctionHouseElement auction in config.AuctionHouses)
            {
                auctions.Add(new Areas.Admin.Models.AuctionViewModel() { Name = auction.Name });
            }

            return PartialView("~/Views/Shared/_Auction.cshtml", auctions);
        }
    }
}