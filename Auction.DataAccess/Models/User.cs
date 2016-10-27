using System;

namespace Auction.DataAccess.Models
{
    public class User : JsonModel
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string Locale { get; set; }

        public string TimezoneId { get; set; }

        public string Picture { get; set; }

        public override string ToString()
        {
            return string.Format("{0}.json", this.GetType().Name);
        }
    }
}
