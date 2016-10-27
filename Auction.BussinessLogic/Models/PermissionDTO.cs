using System;
using System.Collections.Generic;

namespace Auction.BussinessLogic.Models
{
    public class PermissionDTO
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Role Role { get; set; }

        public string AuctionId { get; set; }

        public List<Guid> CategoriesId { get; set; }
    }
}
