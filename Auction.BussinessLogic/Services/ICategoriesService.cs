using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auction.BussinessLogic.Models;

namespace Auction.BussinessLogic.Services
{
    public interface ICategoriesService
    {
        Task<IEnumerable<CategoryDTO>> ShowAwalaibleCategoriesAsync();

        Task EditCategoryAsync(CategoryDTO category);

        Task<CategoryDTO> GetCategoryAsync(Guid? categoryId);

        Task AddCategoryAsync(CategoryDTO category);

        Task RemoveCategoryAsync(Guid? id);

        Task<bool> IsUnigueNameAsync(string name);
    }
}
