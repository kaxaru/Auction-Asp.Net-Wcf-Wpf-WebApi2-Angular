using Auction.DataAccess.Database;

namespace Auction.DataAccess.DbConfig
{
    public interface IDatabaseConfig
    {
        IStorage GetDatabase(Models.Auction auction);

        IStorage GetGlobalDatabase();
    }
}
