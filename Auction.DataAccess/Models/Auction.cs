namespace Auction.DataAccess.Models
{
    public enum TypeAuction
    {
        Sql = 1,
        Json
    }

    public class Auction
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public TypeAuction Type { get; set; }
    }
}
