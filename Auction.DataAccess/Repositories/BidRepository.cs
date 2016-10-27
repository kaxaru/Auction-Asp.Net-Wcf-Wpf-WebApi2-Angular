using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auction.DataAccess.Database;
using Auction.DataAccess.DbConfig;
using Auction.DataAccess.Models;

namespace Auction.DataAccess.Repositories
{
    public class BidRepoditory : IBidRepository
    {
        private readonly IDatabaseConfig _dbconfig;
        private readonly IAuctionProvider _auctionProvider;
        private IStorage _storage;

        public BidRepoditory(IAuctionProvider auctionProvider, IDatabaseConfig dbconfig)
        {
            _auctionProvider = auctionProvider;
            _dbconfig = dbconfig;
        }

        public IStorage Storage
        {
            get { return _storage; }
        }

        public void Configure()
        {
            _storage = _dbconfig.GetDatabase(_auctionProvider.GetAuction());
            _storage.SetModel(new Bid());
        }

        public async Task AddBidAsync(Bid bid)
        {
            await Task.Run(async () =>
            {
                await _storage.AddAsync(bid);
                await _storage.SaveChanges();
            });
        }

        public async Task<IEnumerable<Bid>> GetBidsAsync()
        {
            Task<IEnumerable<Bid>> taskInvoke = Task<IEnumerable<Bid>>.Factory.StartNew(() =>
            {
               return _storage.QueryAsync<Bid>().Result;
            });

            return await taskInvoke;
        }

        public async Task RemoveBidAsync(Bid bid)
        {
            await Task.Run(async () =>
            {
                await _storage.DeleteAsync(bid.Id);
                await _storage.SaveChanges();
            });
        }

        public async Task UpdateBidAsync(Bid bid)
        {
            await Task.Run(async () =>
            {
                await _storage.UpdateAsync(bid);
                await _storage.SaveChanges();
            });
        }

        public async Task<Bid> GetByIdBidAsync(Guid id)
        {
            return await _storage.GetByIdAsync<Bid>(id);
        }

        public async Task<IEnumerable<Bid>> GetBidsAsync(IEnumerable<Product> productList)
        {
            Task<IEnumerable<Bid>> taskInvoke = Task<IEnumerable<Bid>>.Factory.StartNew(() =>
            {
                var bidList = GetBidsAsync();
                List<Bid> currBidList = new List<Bid>();
                foreach (var product in productList)
                {
                    var productBidList = bidList.Result.Where(b => b.ProductId == product.Id);
                    if (productBidList.Count() > 0)
                    {
                        foreach (var productBid in productBidList)
                        {
                            currBidList.Add(productBid);
                        }
                    }
                }

                return currBidList;
            });

            return await taskInvoke;
        }
    }
}
