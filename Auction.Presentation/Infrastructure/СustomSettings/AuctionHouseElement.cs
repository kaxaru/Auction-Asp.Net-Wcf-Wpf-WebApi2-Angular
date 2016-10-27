using System.Configuration;

namespace Auction.Presentation.Infrastructure.СustomSettings
{
    public class AuctionHouseElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("location", IsKey = true, IsRequired = true)]
        public string Location
        {
            get { return (string)this["location"]; }
            set { this["location"] = value; }
        }

        [ConfigurationProperty("type", IsKey = true, IsRequired = true)]
        public string Type
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }
    }
}