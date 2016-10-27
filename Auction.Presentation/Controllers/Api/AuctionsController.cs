using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using Auction.Presentation.Areas.Admin.Models;
using Auction.Presentation.Infrastructure.СustomSettings;

namespace Auction.Presentation.Controllers.Api
{
    public class AuctionsController : ApiController
    {
        private static AuctionHousesSection config = ConfigurationManager.GetSection("AuctionHouses") as AuctionHousesSection;

        [HttpGet]
        [Route("api/auctions")]
        public List<AuctionViewModel> GetAuctions()
        {
            List<AuctionViewModel> auctions = new List<AuctionViewModel>();
            foreach (AuctionHouseElement auction in config.AuctionHouses)
            {
                var type = (TypeAuctionVM)Enum.Parse(typeof(TypeAuctionVM), auction.Type);
                AuctionViewModel auctionVM = new AuctionViewModel()
                {
                    Name = auction.Name,
                    Location = auction.Location,
                    Type = type
                };
                auctions.Add(auctionVM);
            }

            return auctions;
       }
    }
}
