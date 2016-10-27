using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Auction.Presentation.Models
{
    public class ProductsListWithAuction
    {
        public string Auction { get; set; }

        public IEnumerable<ProductViewModel> Products { get; set; }
    }
}