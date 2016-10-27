using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auction.DataAccess.Database;
using Auction.DataAccess.Models;

namespace Auction.DataAccess.Repositories
{
    public interface IBidRepository
    {
        IStorage Storage { get; }

        Task UpdateBidAsync(Bid bid);

        Task AddBidAsync(Bid bid);

        Task RemoveBidAsync(Bid bid);

        Task<Bid> GetByIdBidAsync(Guid id);

        Task<IEnumerable<Bid>> GetBidsAsync();

        Task<IEnumerable<Bid>> GetBidsAsync(IEnumerable<Product> productList);

        void Configure();
    }
}