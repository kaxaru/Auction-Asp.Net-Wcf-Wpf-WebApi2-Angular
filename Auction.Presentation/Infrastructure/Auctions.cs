using System;
using System.Collections.Generic;
using System.Configuration;
using Auction.BussinessLogic.Models;
using Auction.DataAccess.Repositories;
using Auction.Presentation.Infrastructure.СustomSettings;
using Microsoft.Practices.Unity;

namespace Auction.Presentation.Infrastructure
{
    public class Auctions
    {
        private static AuctionHousesSection config = ConfigurationManager.GetSection("AuctionHouses") as AuctionHousesSection;

        public static List<string> GetAuctionsName()
        {
            List<string> auctions = new List<string>();
            foreach (AuctionHouseElement auction in config.AuctionHouses)
            {
                auctions.Add(auction.Name);
            }

            return auctions;
        }

        public static void SetAuction(AuctionHouseElement auction)
        {
            var type = (TypeAuction)Enum.Parse(typeof(TypeAuction), auction.Type);
            var auctionDAL = new Auction.DataAccess.Models.Auction()
            {
                Name = auction.Name,
                Location = auction.Location,
                Type = (DataAccess.Models.TypeAuction)type
            };

            IUnityContainer container = UnityConfig.GetConfiguredContainer();
            var auctionProvider = container.Resolve<DataAccess.Repositories.IAuctionProvider>();
            auctionProvider.Create(auctionDAL);
        }
    }
}