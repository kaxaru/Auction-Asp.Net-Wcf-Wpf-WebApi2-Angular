using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using Auction.BussinessLogic.Services;
using Auction.Presentation.Infrastructure;
using Auction.Presentation.Infrastructure.СustomSettings;
using Auction.Presentation.Models;
using Omu.ValueInjecter;
using WebApi.OutputCache.V2;

namespace Auction.Presentation.Controllers.Api
{
    public class CategoriesController : ApiController
    {
        private static AuctionHousesSection config = ConfigurationManager.GetSection("AuctionHouses") as AuctionHousesSection;
        private readonly DataAccess.Repositories.IAuctionProvider _auctionProvider;
        private ICategoriesService _categoryService;

        public CategoriesController(DataAccess.Repositories.IAuctionProvider auctionProvider, ICategoriesService categoryService)
        {
            _auctionProvider = auctionProvider;
            _categoryService = categoryService;
        }

        [HttpGet]
        [CacheOutput(ClientTimeSpan = 300, ServerTimeSpan = 300)]
        [Route("api/categories")]
        public async Task<Dictionary<string, IEnumerable<CategoryViewModel>>> GetAllCategories()
        {
            var categoryDictionary = new Dictionary<string, IEnumerable<CategoryViewModel>>();
            foreach (AuctionHouseElement auction in config.AuctionHouses)
            {
                Auctions.SetAuction(auction);
                var categoryResult = await _categoryService.ShowAwalaibleCategoriesAsync();
                categoryDictionary.Add(auction.Name, categoryResult.Select(c => Mapper.Map<CategoryViewModel>(c)));
            }

            return categoryDictionary;
        }

        [HttpGet]
        [CacheOutput(ClientTimeSpan = 300, ServerTimeSpan = 300)]
        [Route("api/categories/auctionId")]
        public async Task<IEnumerable<CategoryViewModel>> GetCategoriesFromAuction(string auctionId)
        {
            var auction = config.AuctionHouses.Search(auctionId);
            if (auction != null)
            {
                Auctions.SetAuction(auction);
                var categoriesList = await _categoryService.ShowAwalaibleCategoriesAsync();
                return categoriesList.Select(c => Mapper.Map<CategoryViewModel>(c));
            }

            return null;
        }

        [HttpGet]
        [Route("api/categories/auctionId/id")]
        public async Task<CategoryViewModel> GetCategory(string auctionId, Guid id)
        {
            var auction = config.AuctionHouses.Search(auctionId);
            if (auction != null)
            {
                Auctions.SetAuction(auction);
                var category = await _categoryService.GetCategoryAsync(id);
                if (category == null)
                {
                    return null;
                }

                return Mapper.Map<CategoryViewModel>(category);
            }

            return null;
        }
    }
}
