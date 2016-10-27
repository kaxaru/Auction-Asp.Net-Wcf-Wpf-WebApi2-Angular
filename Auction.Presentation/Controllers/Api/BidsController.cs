using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Auction.BussinessLogic.Services;
using Auction.Presentation.Infrastructure;
using Auction.Presentation.Infrastructure.Authentication;
using Auction.Presentation.Infrastructure.СustomSettings;
using Auction.Presentation.Models;
using Omu.ValueInjecter;

namespace Auction.Presentation.Controllers.Api
{
    public class BidsController : ApiController
    {
        private static AuctionHousesSection config = ConfigurationManager.GetSection("AuctionHouses") as AuctionHousesSection;
        private readonly DataAccess.Repositories.IAuctionProvider _aucProvider;
        private readonly IBidService _bidService;
        private readonly IProductService _productService;
        private readonly CustomUserManager _customUserManager;

        public BidsController(DataAccess.Repositories.IAuctionProvider aucProvider, IBidService bidService, IProductService productService, CustomUserManager customUserManager)
        {
            _aucProvider = aucProvider;
            _bidService = bidService;
            _productService = productService;
            _customUserManager = customUserManager;
        }

        public int BidStep
        {
            get
            {
                int val;
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["bidStep"], out val);
                return val;
            }
        }

        [HttpGet]
        [Route("api/bidOffset")]
        public int GetBidOffset()
        {
            return BidStep;
        }

        [HttpGet]
        [Route("api/bids")]
        public async Task<Dictionary<string, IEnumerable<BidViewModel>>> GetAllBids()
        {
            var bidDictionary = new Dictionary<string, IEnumerable<BidViewModel>>();
            foreach (AuctionHouseElement auction in config.AuctionHouses)
            {
                Auctions.SetAuction(auction);
                bidDictionary.Add(auction.Name, (await _bidService.GetBidsAsync()).Select(c => Mapper.Map<BidViewModel>(c)));
            }

            return bidDictionary;
        }

        [HttpGet]
        [Route("api/bids/auctionId")]
        public async Task<IEnumerable<BidViewModel>> GetBidsFromAuction(string auctionId)
        {
            var auction = config.AuctionHouses.Search(auctionId);
            if (auction != null)
            {
                Auctions.SetAuction(auction);
                var bidList = await _bidService.GetBidsAsync();           
                return bidList.Select(c => Mapper.Map<BidViewModel>(c));
            }

            return null;
        }

        [HttpGet]
        [Route("api/bids/auctionId/categoryId")]
        public async Task<IEnumerable<BidViewModel>> GetBidsFromCategory(string auctionId, Guid id)
        {
            var auction = config.AuctionHouses.Search(auctionId);
            if (auction != null)
            {
                Auctions.SetAuction(auction);
                var bidList = await _bidService.GetBidsAsync(id);
                if (bidList == null)
                {
                    return null;
                }

                return bidList.Select(b => Mapper.Map<BidViewModel>(b));
            }

            return null;
        }

        [HttpPost]
        [Route("api/bids/productList")]
        public async Task<Dictionary<string, IEnumerable<BidViewModel>>> GetBidsFromListProduct([FromBody]IEnumerable<ProductsListWithAuction> products)
        {
            if (products.FirstOrDefault() == null)
            {
                return null;
            }

            Dictionary<string, IEnumerable<BidViewModel>> listlastBids = new Dictionary<string, IEnumerable<BidViewModel>>();
            foreach (var productList in products)
            {
                var auction = config.AuctionHouses.Search(productList.Auction);
                if (auction != null)
                {
                    Auctions.SetAuction(auction);
                   
                    var productsDTO = productList.Products.Select(pr => Mapper.Map<BussinessLogic.Models.ProductDTO>(pr));
                    var listLastBid = await _bidService.ShowLastBidForListProductAsync(productsDTO);
                    var listBidVM = listLastBid.Select(b => Mapper.Map<BidViewModel>(b));

                    listlastBids.Add(productList.Auction, listBidVM);
                }
            }

            return listlastBids;
        }

        [HttpPost]
        [Route("api/bids/auctionId/productId")]
        public async Task<IHttpActionResult> MakeABid([FromBody]BidViewModel bid, string auctionId)
        {
            bid.DateTime = DateTime.Now;
            var auction = config.AuctionHouses.Search(auctionId);
            if (auction == null)
            {
                BadRequest();
            }

            Auctions.SetAuction(auction);
            var product = await _productService.GetProductAsync(bid.ProductId);
            if (product == null)
            {
                BadRequest();
            }

            var user = await _customUserManager.FindByIdAsync(bid.UserId.ToString());
            if (user == null)
            {
                BadRequest();
            }
           
            var lastBid = await _bidService.ShowLastBidForProductAsync(product.Id);
            bid.Id = Guid.NewGuid();
            if (lastBid == null && bid.Price > BidStep)
            {
                await _bidService.AddBidAsync(Mapper.Map<BussinessLogic.Models.Bid>(bid));
                return Ok();
            }

            if (bid.Price >= lastBid.Price + BidStep && lastBid.DateTime < bid.DateTime)
            {
               await _bidService.AddBidAsync(Mapper.Map<BussinessLogic.Models.Bid>(bid));
                return Ok();
            }

            return BadRequest();
        }
    }
}
