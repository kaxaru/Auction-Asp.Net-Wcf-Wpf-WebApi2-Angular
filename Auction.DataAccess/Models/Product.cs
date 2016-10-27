using System;

namespace Auction.DataAccess.Models
{
    public enum State
    {
        Draft = 1,
        Selling,
        Banned,
        OutDated
    }

    public class Product : JsonModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public string Duration { get; set; }

        public int State { get; set; }

        public int StartPrice { get; set;  }

        public Guid CategoryID { get; set; }

        public string Picture { get; set; }

        public Guid UserId { get; set; }

        public override string ToString()
        {
            return string.Format("{0}.json", this.GetType().Name);
        }
    }
}
