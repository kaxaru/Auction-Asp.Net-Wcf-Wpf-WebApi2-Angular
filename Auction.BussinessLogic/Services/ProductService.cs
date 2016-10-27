using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auction.BussinessLogic.Infrastructure;
using Auction.BussinessLogic.Models;
using Auction.DataAccess.Models;
using Auction.DataAccess.Repositories;
using Omu.ValueInjecter;

namespace Auction.BussinessLogic.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task AddProductAsync(ProductDTO product)
        {
            await Task.Run(async () =>
            {
                _productRepository.Configure();

                var productDAL = new Product { Id = Guid.NewGuid() };
                productDAL.InjectFrom(product);
                productDAL.State = (int)product.State;
                productDAL.Duration = product.Duration.ToString();

                await _productRepository.AddProductAsync(productDAL);
            });
        }

        public async Task EditProductAsync(ProductDTO product)
        {
            await Task.Run(async () =>
            {
                if (product == null)
                {
                    throw new ArgumentException(nameof(product));
                }

                _productRepository.Configure();
                var productDAL = await _productRepository.GetProductById(product.Id);
                productDAL.InjectFrom<NoNullsInjection>(product);
                if (default(int) != (int)product.State)
                {
                    productDAL.State = (int)product.State;                
                }

                await _productRepository.UpdateProductAsync(productDAL);
            });
        }

        public async Task<ProductDTO> GetProductAsync(Guid? productId)
        {
            Task<ProductDTO> taskInvoke = Task<ProductDTO>.Factory.StartNew(() =>
            {
                _productRepository.Configure();
                var showingProduct = _productRepository.GetProductsAsync().Result.FirstOrDefault(product => product.Id == productId);

                if (showingProduct == null)
                {
                    throw new ArgumentException("There is no categories with specific id", nameof(showingProduct));
                }

                var productDTO = new ProductDTO() { };
                productDTO.InjectFrom(showingProduct);
                productDTO.State = (Models.State)showingProduct.State;
                productDTO.Duration = TimeSpan.Parse(showingProduct.Duration);

                return productDTO;
            });

            return await taskInvoke;
        }

        public async Task RemoveProductAsync(Guid? productId)
        {
            await Task.Run(async () =>
            {
                if (productId == null)
                {
                    throw new ArgumentException(nameof(productId));
                }

                _productRepository.Configure();
                var product = await _productRepository.GetProductById(productId.GetValueOrDefault());
                await _productRepository.RemoveProductAsync(product);
            });
        }

        public async Task<IEnumerable<ProductDTO>> ShowAwalaibleProductsAsync()
        {
            Task<IEnumerable<ProductDTO>> taskInvoke = Task<IEnumerable<ProductDTO>>.Factory.StartNew(() =>
            {
                _productRepository.Configure();
                var listProduct = GetProductsAsync().Result.Where(p => p.State == Models.State.Selling && p.StartDate.Add(p.Duration) > DateTime.Now);
                foreach (var product in listProduct)
                {
                    product.Duration -= DateTime.Now - product.StartDate;
                }

                return listProduct;
            });
            return await taskInvoke;
        }

        public async Task<IEnumerable<ProductDTO>> ShowProductsFromCategoryAsync(Guid id)
        {
            Task<IEnumerable<ProductDTO>> taskInvoke = Task<IEnumerable<ProductDTO>>.Factory.StartNew(() =>
            {
                _productRepository.Configure();
                var listProducts = ShowAwalaibleProductsAsync();
                return listProducts.Result.Where(p => p.CategoryID == id);
            });

            return await taskInvoke;
        }

        public async Task<IEnumerable<ProductDTO>> ShowProductsOfUserAsync(Guid id)
        {
            Task<IEnumerable<ProductDTO>> taskInvoke = Task<IEnumerable<ProductDTO>>.Factory.StartNew(() =>
            {
                _productRepository.Configure();
                return GetProductsAsync().Result.Where(p => p.UserId == id);
            });

            return await taskInvoke;
        }

        public async Task<IEnumerable<ProductDTO>> ShowProductsForModerationAsync()
        {
            Task<IEnumerable<ProductDTO>> taskInvoke = Task<IEnumerable<ProductDTO>>.Factory.StartNew(() =>
            {
                _productRepository.Configure();
                return GetProductsAsync().Result.Where(p => (p.State == Models.State.Draft));
            });

            return await taskInvoke;
        }

        public async Task<IList<ProductDTO>> ShowProductsForModerationAsync(List<Guid> categories)
        {
            Task<IList<ProductDTO>> taskInvoke = Task<IList<ProductDTO>>.Factory.StartNew(() =>
            {
                _productRepository.Configure();
                var listProducts = new List<ProductDTO>();
                var databaseListProducts = GetProductsAsync().Result;
                databaseListProducts = databaseListProducts.Where(p => (p.State == Models.State.Draft));
                foreach (var category in categories)
                {
                    foreach (var product in databaseListProducts)
                    {
                        if (product.CategoryID == category)
                        {
                            listProducts.Add(product);
                        }
                    }
                }

                return listProducts;
            });

            return await taskInvoke;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsAsync()
        {
            Task<IEnumerable<ProductDTO>> taskInvoke = Task<IEnumerable<ProductDTO>>.Factory.StartNew(() =>
            {
                _productRepository.Configure();
                var products = _productRepository.GetProductsAsync().Result;
                var productsListDTO = new List<ProductDTO>();
                foreach (var product in products)
                {
                    var productDTO = new ProductDTO() { };
                    productDTO.InjectFrom(product);
                    productDTO.State = (Models.State)product.State;
                    productDTO.Duration = TimeSpan.Parse(product.Duration);
                    productsListDTO.Add(productDTO);
                }

                return productsListDTO;
            });

            return await taskInvoke;
        }

        public async Task<IEnumerable<ProductDTO>> ShowAwalaibleProductsWithTimeOffAsync()
        {
            Task<IEnumerable<ProductDTO>> taskInvoke = Task<IEnumerable<ProductDTO>>.Factory.StartNew(() =>
            {
                _productRepository.Configure();
                var listProduct = GetProductsAsync().Result.Where(p => p.State == Models.State.Selling && p.StartDate.Add(p.Duration) < DateTime.Now);
                var list = listProduct.ToList();
                foreach (var product in list)
                {
                    product.Duration -= DateTime.Now - product.StartDate;
                }           

            return list;
            });

            return await taskInvoke;
        }    
    }
}
