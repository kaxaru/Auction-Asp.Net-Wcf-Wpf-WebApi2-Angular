using Auction.DataAccess.Database;
using Auction.DataAccess.Models;

namespace Auction.DataAccess.DatabaseContext
{
    public interface ISqlStorage
    {
        System.Data.Entity.DbSet<Category> Categories { get; set; }

        void GetConnectionString(string connection);

        IStorage SetConnectionString(string connection);

        bool IsFileExists(string filePath);
    }
}
