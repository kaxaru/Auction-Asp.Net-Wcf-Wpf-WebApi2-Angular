using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auction.BussinessLogic.Models;

namespace Auction.BussinessLogic.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> ShowAwalaibleProductsAsync();

        Task<IEnumerable<ProductDTO>> ShowAwalaibleProductsWithTimeOffAsync();

        Task<IEnumerable<ProductDTO>> ShowProductsFromCategoryAsync(Guid id);

        Task<IEnumerable<ProductDTO>> ShowProductsOfUserAsync(Guid id);

        Task<IEnumerable<ProductDTO>> ShowProductsForModerationAsync();

        Task<IList<ProductDTO>> ShowProductsForModerationAsync(List<Guid> categories);

        Task EditProductAsync(ProductDTO product);

        Task<ProductDTO> GetProductAsync(Guid? productId);

        Task<IEnumerable<ProductDTO>> GetProductsAsync();

        Task AddProductAsync(ProductDTO product);

        Task RemoveProductAsync(Guid? id);
    }
}
