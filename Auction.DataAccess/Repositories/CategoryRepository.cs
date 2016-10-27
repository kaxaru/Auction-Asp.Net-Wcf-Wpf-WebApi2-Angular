using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auction.DataAccess.Database;
using Auction.DataAccess.DbConfig;
using Auction.DataAccess.Models;

namespace Auction.DataAccess.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDatabaseConfig _dbconfig;
        private readonly IAuctionProvider _auctionProvider;
        private IStorage _storage;

        public CategoryRepository(IDatabaseConfig dbconfig, IAuctionProvider auction)
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
            _storage.SetModel(new Category());
        }

        public async Task AddCategoryAsync(Category category)
        {
            await Task.Run(async () =>
            {
                await _storage.AddAsync(category);
                await _storage.SaveChanges();
            });
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            /* Task<IEnumerable<Category>> taskInvoke = Task<IEnumerable<Category>>.Factory.StartNew(() =>
             {
                 return _storage.QueryAsync<Category>().Result;
             });*/

            return await _storage.QueryAsync<Category>();
        }

        public async Task RemoveCategoryAsync(Category category)
        {
            await Task.Run(async () =>
            {
                await _storage.DeleteAsync(category.Id);
                await _storage.SaveChanges();
            });
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            await Task.Run(async () =>
            {
                await _storage.UpdateAsync(category);
                await _storage.SaveChanges();
            });
        }

        public async Task<Category> GetByIdAsync(Guid id)
        {
            return await _storage.GetByIdAsync<Category>(id);
        }
    }
}
