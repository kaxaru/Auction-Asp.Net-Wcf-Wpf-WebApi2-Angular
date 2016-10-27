namespace Auction.DataAccess.Repositories
{
    public interface IAuctionProvider
    {
        void Create(Models.Auction auction);

        Models.Auction GetAuction();
    }
}
