using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Auction.Presentation.Models
{
    public class ProductClientViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public double Duration { get; set; }

        public Guid CategoryID { get; set; }

        public string Picture { get; set; }

        public int StartPrice { get; set; }
    }
}