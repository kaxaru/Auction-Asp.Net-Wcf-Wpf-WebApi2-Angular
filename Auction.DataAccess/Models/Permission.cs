using System;
using System.Collections.Generic;

namespace Auction.DataAccess.Models
{
    public class Permission : JsonModel
    {
        public Guid UserId { get; set; }

        public int Role { get; set; }

        public string AuctionId { get; set; }

        public List<Guid> CategoriesId { get; set; }

        public override string ToString()
        {
            return string.Format("{0}.json", this.GetType().Name);
        }
    }
}
