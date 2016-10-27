using System;

namespace Auction.BussinessLogic.Models
{
    public class Bid
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid ProductId { get; set; }

        public DateTime DateTime { get; set; }

        public int Price { get; set; }
    }
}
