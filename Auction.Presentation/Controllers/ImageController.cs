using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using Auction.BussinessLogic.Services;
using Auction.Presentation.Helpers;
using Auction.Presentation.Infrastructure;
using Auction.Presentation.Infrastructure.Authentication;
using Auction.Presentation.Infrastructure.СustomSettings;

namespace Auction.Presentation.Controllers
{
    public class ImageController : Controller
    {
        private static AuctionHousesSection config = ConfigurationManager.GetSection("AuctionHouses") as AuctionHousesSection;
        private readonly IProductService _productService;
        private readonly DataAccess.Repositories.IAuctionProvider _auctionProvider;
        private readonly CustomUserManager _customUserManager;

        public ImageController(DataAccess.Repositories.IAuctionProvider auction, IProductService productService, CustomUserManager customUserManager)
        {
            _auctionProvider = auction;
            _productService = productService;
            _customUserManager = customUserManager;
        }

        public async Task<ActionResult> RenderImageProduct(Guid id, string auctionId)
        {
            var auction = config.AuctionHouses.Search(auctionId);
            if (auction == null)
            {
               ////error
            }

            Auctions.SetAuction(auction);
            var product = await _productService.GetProductAsync(id);
            var imageByteProduct = ImageHelper.Base64ToImage(product.Picture);
            return File(imageByteProduct, "image/png");
        }

        public async Task<ActionResult> RenderImageUser(Guid id)
        {
            var user = await Task.Run(() => _customUserManager.FindByIdAsync(id.ToString()).Result);
            var imageByteUser = ImageHelper.Base64ToImage(user.Picture);
            return File(imageByteUser, "image/png");
        }
    }
}