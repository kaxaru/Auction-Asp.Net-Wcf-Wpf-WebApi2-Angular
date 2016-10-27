using System.Data.Entity;
using Auction.DataAccess.Models;

namespace Auction.DataAccess.DatabaseContext
{
    public class AuctionDbInit : DropCreateDatabaseAlways<AuctionDbContext>
    {
        private DataBinding db;

        protected override void Seed(AuctionDbContext context)
        {
            db = Data.DataInit.GetData(); /*db.Categories.Select(el => context.Categories.Add(el));*/
            foreach (var category in db.Categories)
            {
                context.Categories.Add(category);
            }

            context.SaveChanges();
            foreach (var product in db.Products)
            {
                context.Products.Add(product);
            }

            context.SaveChanges();
        }
    }
}
