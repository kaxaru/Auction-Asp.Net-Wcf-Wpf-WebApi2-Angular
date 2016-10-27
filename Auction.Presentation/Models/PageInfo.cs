using System;

namespace Auction.Presentation.Models
{
    public class PageInfo
    {
        private const int Page = 5; 

        public int PageNumber { get; set; }

        public int PageSize
        {
            get
            {
                return Page;
            }
        }

        public int TotalItems { get; set; }

        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / PageSize); }
        }
    }
}