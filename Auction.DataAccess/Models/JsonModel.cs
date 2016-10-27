using System;

namespace Auction.DataAccess.Models
{
    public class JsonModel
    {
        public Guid Id { get; set; }

        public DateTime UpdateOn
        {
            get { return DateTime.Now; }

            set { value = UpdateOn; }
        }
    }
}
