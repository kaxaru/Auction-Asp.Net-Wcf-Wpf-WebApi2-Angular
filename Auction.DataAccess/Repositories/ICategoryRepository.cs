using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auction.DataAccess.Database;
using Auction.DataAccess.Models;

namespace Auction.DataAccess.Repositories
{
    public interface ICategoryRepository
    {
        IStorage Storage { get; }

        Task<IEnumerable<Category>> GetCategoriesAsync();

        Task UpdateCategoryAsync(Category category);

        Task AddCategoryAsync(Category category);

        Task RemoveCategoryAsync(Category category);

        Task<Category> GetByIdAsync(Guid id);

        void Configure();
    }
}
