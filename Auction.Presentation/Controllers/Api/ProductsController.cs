using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Auction.BussinessLogic.Models;
using Auction.BussinessLogic.Services;
using Auction.Presentation.Infrastructure;
using Auction.Presentation.Infrastructure.СustomSettings;
using Auction.Presentation.Models;
using Omu.ValueInjecter;
using WebApi.OutputCache.V2;

namespace Auction.Presentation.Controllers.Api
{
    public class ProductsController : ApiController
    {
        private static AuctionHousesSection config = ConfigurationManager.GetSection("AuctionHouses") as AuctionHousesSection;
        private readonly DataAccess.Repositories.IAuctionProvider _auctionProvider;
        private readonly ICategoriesService _categoryService;
        private readonly IProductService _productService;

        public ProductsController(DataAccess.Repositories.IAuctionProvider auctionProvider, ICategoriesService categoryService, IProductService productService)
        {
            _auctionProvider = auctionProvider;
            _categoryService = categoryService;
            _productService = productService;
        }

        [HttpGet]
        [CacheOutput(ClientTimeSpan = 600, ServerTimeSpan = 600)]
        [Route("api/products")]
        public async Task<Dictionary<string, IEnumerable<ProductViewModel>>> GetAllProducts()
        {
            var productsDictionary = new Dictionary<string, IEnumerable<ProductViewModel>>();
            foreach (AuctionHouseElement auction in config.AuctionHouses)
            {
                Auctions.SetAuction(auction);
                var productsResult = await _productService.GetProductsAsync();
                productsDictionary.Add(auction.Name,  productsResult.Select(p => Mapper.Map<ProductViewModel>(p)));
            }

            return productsDictionary;
        }

        [HttpGet]
        [CacheOutput(ClientTimeSpan = 300, ServerTimeSpan = 300)]
        [Route("api/products/auctionId")]
        public async Task<IEnumerable<ProductViewModel>> GetProductsFromAuction(string auctionId)
        {
            var auction = config.AuctionHouses.Search(auctionId);
            if (auction != null)
            {
                Auctions.SetAuction(auction);
                return (await _productService.GetProductsAsync()).Select(p => Mapper.Map<ProductViewModel>(p));
            }

            return null;
        }

        [HttpGet]
        [CacheOutput(ClientTimeSpan = 300, ServerTimeSpan = 300)]
        [Route("api/products/auctionId/categoryId")]
        public async Task<IEnumerable<ProductViewModel>> GetProductsFromCategory(string auctionId, Guid categoryId)
        {
            var auction = config.AuctionHouses.Search(auctionId);
            if (auction != null)
            {
                Auctions.SetAuction(auction);
                return (await _productService.ShowProductsFromCategoryAsync(categoryId)).Select(p => Mapper.Map<ProductViewModel>(p));
            }

            return null;
        }

        [HttpGet]
        [Route("api/products/auctionId/categoryId/id")]
        public async Task<ProductViewModel> GetProduct(string auctionId, Guid productId)
        {
            var auction = config.AuctionHouses.Search(auctionId);
            if (auction != null)
            {
                Auctions.SetAuction(auction);
                var productDTO = await _productService.GetProductAsync(productId);
                return Mapper.Map<ProductViewModel>(productDTO);
            }

            return null;
        }

        [HttpPost]
        [Route("api/products/auctionId/categoryId/id")]
        public async Task<IHttpActionResult> CreateProduct([FromBody]ProductViewModel product,  string auctionId)
        {
            var auction = config.AuctionHouses.Search(auctionId);
            if (auction == null)
            {
                return NotFound();
            }

            Auctions.SetAuction(auction);
            ////fix for user 
            product.Id = Guid.NewGuid();
            product.State = State.Draft;
            product.StartDate = DateTime.Now;

            var productDTO = Mapper.Map<ProductDTO>(product);
            await _productService.AddProductAsync(productDTO);
            return Ok();
        }
    }
}
