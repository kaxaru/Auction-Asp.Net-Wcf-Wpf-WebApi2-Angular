using System.Collections.Generic;

namespace Auction.Presentation.Models
{
    public class ProductsWithBidViewModel
    {
        public IEnumerable<ProductClientViewModel> Products { get; set; }

        public IEnumerable<BidViewModel> Bids { get; set; }

        public PageInfo PageInfo { get; set; }

        public IEnumerable<ProfileViewModel> Users { get; set; }
    }
}