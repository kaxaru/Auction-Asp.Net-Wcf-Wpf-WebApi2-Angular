using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auction.BussinessLogic.Models;

namespace Auction.BussinessLogic.Services
{
    public interface IBidService 
    {
        Task UpdateBidAsync(Bid bid);

        Task AddBidAsync(Bid bid);

        Task RemoveBidAsync(Bid bid);

        Task<IEnumerable<Bid>> GetBidsAsync();

        Task<IEnumerable<Bid>> GetBidsAsync(Guid categoryId);

        Task<Bid> ShowLastBidForProductAsync(Guid id);

        Task<IList<Bid>> ShowLastBidForListProductAsync(IEnumerable<ProductDTO> listProduct);

        Task<IList<Bid>> WinnerListProductAsync(Guid userId);
    }
}
