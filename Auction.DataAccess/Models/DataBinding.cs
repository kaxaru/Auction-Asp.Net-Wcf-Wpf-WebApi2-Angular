using System.Collections.Generic;

namespace Auction.DataAccess.Models
{
    public class DataBinding
    {
        public IList<Category> Categories { get; set; }

        public IList<Product> Products { get; set; }

        public IList<User> Users { get; set; }
    }
}
