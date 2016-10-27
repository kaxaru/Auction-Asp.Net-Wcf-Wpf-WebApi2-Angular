using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Models;

namespace WPF.Service
{
    public interface IBidService
    {
        Task<IEnumerable<Bid>> GetBids(Auction auction);

        Task<int> GetBidOffset();

        Task<bool> MakeBid(Bid bid, Auction auction);
    }
}
