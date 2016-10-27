using System;
using System.Collections.Generic;
using Auction.Presentation.Models;

namespace Auction.Presentation.Areas.Admin.Models
{
    public class PermissionViewModel
    {
        public Guid UserId { get; set; }

        public Role Role { get; set; }

        public string AuctionId { get; set; }

        public List<Guid> CategoriesId { get; set; }
    }
}