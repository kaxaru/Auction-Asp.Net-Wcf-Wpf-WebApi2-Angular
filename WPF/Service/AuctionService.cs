using System.Collections.Generic;
using System.Threading.Tasks;
using WPF.Models;

namespace WPF.Service
{
    public class AuctionService : ServiceBase, IAuctionService
    {
        public async Task<IEnumerable<Auction>> GetAuctions()
        {
            return await ApiGetAsync<Auction>("auctions", new string[] { string.Empty });
        }
    }
}
