using System.Configuration;

namespace Auction.Presentation.Infrastructure.СustomSettings
{
    public class AuctionHousesSection : ConfigurationSection
    {
        [ConfigurationProperty("auctions", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(AuctionHousesElement), AddItemName = "auction", ClearItemsName = "clear", RemoveItemName = "remove")]
        public AuctionHousesElement AuctionHouses
        {
            get { return (AuctionHousesElement)this["auctions"]; }
            set { this["auctions"] = value; }
        }
    }
}