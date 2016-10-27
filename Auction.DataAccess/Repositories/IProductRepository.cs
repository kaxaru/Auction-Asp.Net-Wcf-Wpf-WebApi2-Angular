using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auction.DataAccess.Database;
using Auction.DataAccess.Models;

namespace Auction.DataAccess.Repositories
{
    public interface IProductRepository
    {
        IStorage Storage { get; }

        Task<IEnumerable<Product>> GetProductsAsync();

        Task UpdateProductAsync(Product product);

        Task AddProductAsync(Product product);

        Task RemoveProductAsync(Product product);

        Task<Product> GetProductById(Guid id);

        void Configure();
    }
}
