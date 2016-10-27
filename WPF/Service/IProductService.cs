using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Models;
using WPF.ViewModels;

namespace WPF.Service
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProducts();

        Task<IEnumerable<Product>> GetProducts(Auction auction);

        Task<IEnumerable<Product>> GetProducts(Auction auction, Guid? id);

        Task CreateProduct(Product product, Auction auction);
    }
}
