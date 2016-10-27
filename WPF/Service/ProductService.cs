using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WPF.Models;

namespace WPF.Service
{
    public class ProductService : ServiceBase, IProductService
    {
        public async Task<IEnumerable<Product>> GetProducts(Auction auction)
        {
            return await ApiGetAsync<Product>("products", new string[] { "auctionId" }, new Dictionary<string, string> { { "auctionId", auction.Name } });
        }

        public async Task<IEnumerable<Product>> GetProducts(Auction auction, Guid? id)
        {
            return await ApiGetAsync<Product>("products", new string[] { "auctionId", "categoryId" }, new Dictionary<string, string> { { "auctionId", auction.Name }, { "categoryId", id.ToString() } });
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            IEnumerable<Product> result = await ApiGetAsync<Product>("products", new string[] { "auctionId" });
            return result;
        }

        public async Task CreateProduct(Product product, Auction auction)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Properties.Settings.Default.BaseUrl);
                var result = await client.PostAsync($"products/auctionId/categoryId/id?auctionId={auction.Name}", new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json"));
            }
        }
    }
}
