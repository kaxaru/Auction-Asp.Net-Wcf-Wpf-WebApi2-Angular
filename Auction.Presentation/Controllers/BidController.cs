using System;
using System.Configuration;
using System.Dynamic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Auction.BussinessLogic.Models;
using Auction.BussinessLogic.Services;
using Auction.Presentation.Helpers;
using Auction.Presentation.Infrastructure;
using Auction.Presentation.Infrastructure.Authentication;
using Auction.Presentation.Infrastructure.СustomSettings;
using Auction.Presentation.Models;
using Newtonsoft.Json;
using Omu.ValueInjecter;

namespace Auction.Presentation.Controllers
{
    public class BidController : Controller
    {
        private static AuctionHousesSection config = ConfigurationManager.GetSection("AuctionHouses") as AuctionHousesSection;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly DataAccess.Repositories.IAuctionProvider _aucProvider;
        private readonly IBidService _bidService;
        private readonly IProductService _productService;
        private readonly CustomUserManager _customUserManager;

        public BidController(DataAccess.Repositories.IAuctionProvider aucProvider, IBidService bidService, IProductService productService, CustomUserManager customUserManager)
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
                int.TryParse(ConfigurationManager.AppSettings["bidStep"], out val);
                return val;
            }
        }

        public async Task<JsonResult> Create(string data)
        {           
            dynamic answerFromServer = new ExpandoObject();
            answerFromServer.errorMessage = string.Empty;
            answerFromServer.response = string.Empty;
            answerFromServer.userName = string.Empty;
            BidViewModel bid = new BidViewModel() { Id = Guid.NewGuid() };
            if (Request.IsAjaxRequest())
            {
                try
                {                   
                    dynamic bidClient = JsonConvert.DeserializeObject(data);

                    var auctionModel = config.AuctionHouses.Search(bidClient.auction.ToString());
                    if (auctionModel != null)
                    {
                        Auctions.SetAuction(auctionModel);
                    }

                    if ((bidClient.userId == null) || (bidClient.productId == null))
                    {
                        return answerFromServer.errorMessage = "userId or productId not found in database";
                    }

                    bid.UserId = (bidClient.userId == null) ? Guid.Empty : Guid.Parse(bidClient.userId.ToString());
                    var user = await _customUserManager.FindByIdAsync(bid.UserId.ToString());
                    bid.DateTime = DateTime.Now.Add(TimeZoneHelper.ConverTimeToServer(user.TimezoneId));
                    bid.Price = Convert.ToInt32(bidClient.price);
                    bid.ProductId = Guid.Parse(bidClient.productId.ToString());
                    var lastBid = await _bidService.ShowLastBidForProductAsync(bid.ProductId);
                    ProductDTO product = new ProductDTO();
                    if (lastBid == null)
                    {
                        product = await _productService.GetProductAsync(bid.ProductId);
                    }

                    if ((lastBid == null && bid.Price >= product.StartPrice + BidStep) || (lastBid.DateTime < bid.DateTime && lastBid.Price + BidStep <= bid.Price && bid.UserId != Guid.Empty))
                    {                     
                        answerFromServer.userName = user.FullName;
                        answerFromServer.response = bid;
                        _logger.Info(string.Format("user make to bid on product  - {0}, username -{1}, timezone - {2}, auction - {3} , productId - {4}", user.Id, user.FullName, user.TimezoneId, auctionModel, product.Id));
                        await _bidService.AddBidAsync(Mapper.Map<Bid>(bid));
                    }
                    else
                    {
                        answerFromServer.errorMessage = "Not valid params. Obviously this problem to start when not correct price/data ";
                    } 
                }
                catch
                {
                    answerFromServer.errorMessage = "not valid data";
                }
            }

            return Json(JsonConvert.SerializeObject(answerFromServer, Formatting.None));
        }
    }
}