using System.Collections.Generic;
using System.Threading.Tasks;
using WPF.Models;

namespace WPF.Service
{
    public class CategoryService : ServiceBase, ICategoryService
    {
        public async Task<IEnumerable<Category>> GetCategories(Auction auction)
        {
            return await ApiGetAsync<Category>("categories", new string[] { "auctionId" }, new Dictionary<string, string> { { "auctionId", auction.Name } });
        }
    }
}
