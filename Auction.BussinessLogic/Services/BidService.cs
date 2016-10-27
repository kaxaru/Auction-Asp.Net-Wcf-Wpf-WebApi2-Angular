using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auction.BussinessLogic.Infrastructure.Extendion;
using Auction.BussinessLogic.Models;
using Auction.DataAccess.Database;
using Auction.DataAccess.Models;
using Auction.DataAccess.Repositories;
using Omu.ValueInjecter;

namespace Auction.BussinessLogic.Services
{
    public class BidService : IBidService
    {
        private readonly IBidRepository _bidRepository;
        private readonly IProductService _productService;

        public BidService(IBidRepository bidRepository, IProductService productService)
        {
            _bidRepository = bidRepository;
            _productService = productService;        
        }

        public IStorage Storage
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public async Task<Models.Bid> ShowLastBidForProductAsync(Guid productId)
        {
            Task<Models.Bid> taskInvoke = Task<Models.Bid>.Factory.StartNew(() =>
            {
                _bidRepository.Configure();
                var listBid = _bidRepository.GetBidsAsync().Result.Where(b => b.ProductId == productId);
                if (listBid.IsNullOrEmpty())
                {
                    return null;
                }

                var listBidDTO = listBid.Select(b => Mapper.Map<Models.Bid>(b));

                return listBidDTO.MaxBy(b => b.Price);
            });

            return await taskInvoke;
        }

        public async Task<IList<Models.Bid>> ShowLastBidForListProductAsync(IEnumerable<ProductDTO> listProduct)
        {
            Task<IList<Models.Bid>> taskInvoke = Task<IList<Models.Bid>>.Factory.StartNew(() =>
            {
                IList<Models.Bid> listBid = new List<Models.Bid>();
                foreach (var product in listProduct)
                {
                    var lastBidModel = ShowLastBidForProductAsync(product.Id).Result;
                    if (lastBidModel != null)
                    {
                        listBid.Add(lastBidModel);
                    }
                }

                return listBid;
            });

            return await taskInvoke;
        }

        public async Task<IList<Models.Bid>> WinnerListProductAsync(Guid userId)
        {
            Task<IList<Models.Bid>> taskInvoke = Task<IList<Models.Bid>>.Factory.StartNew(() =>
            {
                _bidRepository.Configure();
                var listUserBid = _bidRepository.GetBidsAsync().Result.Where(b => b.UserId == userId);
                var listUserBidDTO = listUserBid.Select(b => Mapper.Map<Models.Bid>(b));
                List<Models.Bid> allLastBid = new List<Models.Bid>();
                foreach (var bid in listUserBidDTO)
                {
                    if (!allLastBid.IsNullOrEmpty())
                    {
                        var el = allLastBid.FirstOrDefault(b => b.ProductId == bid.ProductId);
                        if (el != null)
                        {
                            listUserBid.GetEnumerator().MoveNext();
                        }
                        else
                        {
                            var productList = listUserBidDTO.Where(b => b.ProductId == bid.ProductId);
                            allLastBid.Add(productList.MaxBy(b => b.Price));
                        }
                    }
                    else
                    {
                        if (listUserBid.GetEnumerator().Current == null)
                        {                           
                            allLastBid.Add(bid);
                        }
                        else
                        {
                            var productList = listUserBidDTO.Where(b => b.ProductId == bid.ProductId).ToList();
                            allLastBid.Add(productList.MaxBy(b => b.Price));
                        }
                    }
                }

                foreach (var bid in allLastBid)
                {
                    if (!ShowLastBidForProductAsync(bid.ProductId).Id.Equals(bid.Id))
                    {
                        allLastBid.RemoveAll(b => b.ProductId == bid.ProductId);
                    }
                }

                return allLastBid;
            });

            return await taskInvoke;
        }

        public async Task UpdateBidAsync(Models.Bid bid)
        {
            await Task.Run(async () =>
            {
                _bidRepository.Configure();
               var bidDAL = await _bidRepository.GetByIdBidAsync(bid.Id);
                bidDAL.InjectFrom(bid);
               await _bidRepository.UpdateBidAsync(bidDAL);
            });
        }

        public async Task AddBidAsync(Models.Bid bid)
        {
            await Task.Run(async () =>
            {
                _bidRepository.Configure();
                var bidDAL = new DataAccess.Models.Bid()
                {
                    Id = (bid.Id != Guid.Empty) ? bid.Id : Guid.NewGuid()
                };
                bidDAL.InjectFrom(bid);

                await _bidRepository.AddBidAsync(bidDAL);
            });
        }

        public async Task RemoveBidAsync(Models.Bid bid)
        {
            await Task.Run(async () =>
            {
                _bidRepository.Configure();
                var bidDAL = new DataAccess.Models.Bid() { };
                bidDAL.InjectFrom(bid);
                await _bidRepository.RemoveBidAsync(bidDAL);
            });
        }

        public async Task<IEnumerable<Models.Bid>> GetBidsAsync()
        {
            Task<IEnumerable<Models.Bid>> taskInvoke = Task<IEnumerable<Models.Bid>>.Factory.StartNew(() =>
            {
                _bidRepository.Configure();
                var bids = _bidRepository.GetBidsAsync().Result;
                return bids.Select(b => Mapper.Map<Models.Bid>(b));
            });

            return await taskInvoke;
        }

        public void Configure()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Models.Bid>> GetBidsAsync(Guid categoryId)
        {
            Task<IEnumerable<Models.Bid>> taskInvoke = Task<IEnumerable<Models.Bid>>.Factory.StartNew(() =>
            {
                var productListDTO = _productService.ShowProductsFromCategoryAsync(categoryId).Result;
                var productsList = new List<Product>();
                foreach (var productDTO in productListDTO)
                {
                    var product = new Product() { };
                    product.InjectFrom(productDTO);
                    product.State = (int)productDTO.State;
                    product.Duration = productDTO.Duration.ToString();
                    productsList.Add(product);
                }
                
                return _bidRepository.GetBidsAsync(productsList).Result.Select(b => Mapper.Map<Models.Bid>(b));
            });

            return await taskInvoke;
        }
    }
}
