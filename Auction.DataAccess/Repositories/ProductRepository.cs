using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auction.DataAccess.Database;
using Auction.DataAccess.DbConfig;
using Auction.DataAccess.Models;

namespace Auction.DataAccess.Repositories
{
    public class JSONProductRepository : IProductRepository
    {
        private readonly IDatabaseConfig _dbconfig;
        private readonly IAuctionProvider _auctionProvider;
        private IStorage _storage;

        public JSONProductRepository(IDatabaseConfig dbconfig, IAuctionProvider auction)
        {
            _dbconfig = dbconfig;
            _auctionProvider = auction;
        }

        public IStorage Storage
        {
            get { return _storage; }
        }

        public void Configure()
        {
            _storage = _dbconfig.GetDatabase(_auctionProvider.GetAuction());
            if (_storage != null)
            {
                _storage.SetModel(new Product());
            }
        }

        public async Task AddProductAsync(Product product)
        {
            await Task.Run(async () =>
            {
                await _storage.AddAsync(product);
                await _storage.SaveChanges();
            });
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            Task<IEnumerable<Product>> taskInvoke = Task<IEnumerable<Product>>.Factory.StartNew(() =>
            {
                ////var listProduct = _storage.Query<Product>().Result;
                /*var listProductDTO = listProduct.Result.Select(m => new Product()
                {
                    Id = m.Id,
                    State = (Models.State)m.State,
                    CategoryID = m.CategoryID,
                    Description = m.Description,
                    Duration = TimeSpan.Parse(m.Duration),
                    Name = m.Name,
                    Picture = m.Picture,
                    StartDate = m.StartDate,
                    StartPrice = m.StartPrice,
                    UserId = m.UserId
                });*/
                return _storage.QueryAsync<Product>().Result;
            });

            return await taskInvoke;
        }

        public async Task RemoveProductAsync(Product product)
        {
            await Task.Run(async () =>
            {
                await _storage.DeleteAsync(product.Id);
                await _storage.SaveChanges();
            });
        }

        public async Task UpdateProductAsync(Product product)
        {
            await Task.Run(async () =>
            {
                await _storage.UpdateAsync(product);
                await _storage.SaveChanges();
            });
        }

        public async Task<Product> GetProductById(Guid id)
        {
            return await _storage.GetByIdAsync<Product>(id);
        }
    }
}