using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WPF.Models;

namespace WPF.Service
{
    public class BidService : ServiceBase, IBidService
    {
        public async Task<int> GetBidOffset()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Properties.Settings.Default.BaseUrl);
                var result = client.GetAsync("bidOffset").Result;
                var content = await result.Content.ReadAsStringAsync();
                return int.Parse(content);
            }
        }

        public async Task<IEnumerable<Bid>> GetBids(Auction auction)
        {
            return await ApiGetAsync<Bid>("bids", new string[] { "auctionId" }, new Dictionary<string, string> { { "auctionId", auction.Name } });
        }

        public async Task<IEnumerable<Bid>> GetBids(Auction auction, Category categoryId)
        {
            return await ApiGetAsync<Bid>("bids", new string[] { "auctionId" }, new Dictionary<string, string> { { "auctionId", auction.Name } });
        }

        public async Task<bool> MakeBid(Bid bid, Auction auction)
        {
            var task = Task<bool>.Run(async () =>
            {
               using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Properties.Settings.Default.BaseUrl);
                    var result = await client.PostAsync($"bids/auctionId/productId?auctionId={auction.Name}", new StringContent(JsonConvert.SerializeObject(bid), Encoding.UTF8, "application/json"));
                    if (result.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            });
            return await task;
        }
    }
}
