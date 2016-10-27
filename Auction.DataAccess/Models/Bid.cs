using System;

namespace Auction.DataAccess.Models
{
    public class Bid : JsonModel
    {
        public Guid UserId { get; set; }

        public Guid ProductId { get; set; }

        public DateTime DateTime { get; set; }

        public int Price { get; set; }

        public override string ToString()
        {
            return string.Format("{0}.json", this.GetType().Name);
        }
    }
}
