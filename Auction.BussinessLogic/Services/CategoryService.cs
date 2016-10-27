using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auction.BussinessLogic.Infrastructure;
using Auction.BussinessLogic.Models;
using Auction.DataAccess.Models;
using Auction.DataAccess.Repositories;
using Omu.ValueInjecter;

namespace Auction.BussinessLogic.Services
{
    public class CategoryService : ICategoriesService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task AddCategoryAsync(CategoryDTO category)
        {
            await Task.Run(async () =>
            {
                if (category == null)
                {
                    throw new ArgumentNullException(nameof(category));
                }

                _categoryRepository.Configure();

                var categoryDAL = new Category { Id = Guid.NewGuid() };
                categoryDAL.InjectFrom(category);

                await _categoryRepository.AddCategoryAsync(categoryDAL);
            });
        }

        public async Task EditCategoryAsync(CategoryDTO category)
        {
            await Task.Run(async () =>
            {
                if (category == null)
                {
                    throw new ArgumentNullException(nameof(category));
                }

                _categoryRepository.Configure();

                var categoryDAL = await _categoryRepository.GetByIdAsync(category.Id);
                categoryDAL.InjectFrom<NoNullsInjection>(category);

                await _categoryRepository.UpdateCategoryAsync(categoryDAL);
            });
        }

        public async Task<CategoryDTO> GetCategoryAsync(Guid? categoryId)
        {
            Task<CategoryDTO> taskInvoke = Task<CategoryDTO>.Factory.StartNew(() =>
            {
                if (categoryId == null)
                {
                    throw new ArgumentNullException(nameof(categoryId));
                }

                _categoryRepository.Configure();
                var showingCategories = _categoryRepository.GetCategoriesAsync().Result.FirstOrDefault(category => category.Id == categoryId);                
                if (showingCategories == null)
                {
                    throw new ArgumentException("There is no categories with specific id", nameof(categoryId));
                }

                CategoryDTO categoryDTO = new CategoryDTO() { };
                categoryDTO.InjectFrom(showingCategories);
                return categoryDTO;
            });

            return await taskInvoke;
        }

        public async Task<bool> IsUnigueNameAsync(string name)
        {
            Task<bool> taskInvoke = Task<bool>.Factory.StartNew(() =>
            {
                _categoryRepository.Configure();
                return ShowAwalaibleCategoriesAsync().Result.FirstOrDefault(c => c.Name == name) == null ? true : false;
            });
            return await taskInvoke;
        }

        public async Task RemoveCategoryAsync(Guid? categoryId)
        {
            await Task.Run(async () =>
            {
                if (categoryId == null)
                {
                    throw new ArgumentNullException(nameof(categoryId));
                }

                _categoryRepository.Configure();

                var category = await _categoryRepository.GetByIdAsync(categoryId.GetValueOrDefault());
                await _categoryRepository.RemoveCategoryAsync(category);
            });               
        }

        public async Task<IEnumerable<CategoryDTO>> ShowAwalaibleCategoriesAsync()
        {
            Task<IEnumerable<CategoryDTO>> taskInvoke = Task<IEnumerable<CategoryDTO>>.Factory.StartNew(() =>
            {
                _categoryRepository.Configure();
                var allCategories = _categoryRepository.GetCategoriesAsync().Result.Select(c => Mapper.Map<CategoryDTO>(c));

                return allCategories;
            });

            return await taskInvoke;
        }
    }
}
