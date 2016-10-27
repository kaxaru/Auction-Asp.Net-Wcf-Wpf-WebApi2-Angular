using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Auction.Presentation.Models
{
    public class PermissionViewModel
    {
        public Guid UserId { get; set; }

        public int Role { get; set; }

        public string AuctionId { get; set; }

        public List<Guid> CategoriesId { get; set; }
    }
}