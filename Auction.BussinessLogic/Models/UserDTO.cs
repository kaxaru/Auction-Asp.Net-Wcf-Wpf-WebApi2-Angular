using System;

namespace Auction.BussinessLogic.Models
{
    public class UserDTO
    {
        public Guid Id { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string Locale { get; set; }

        public string TimezoneId { get; set; }

        public int Role { get; set; }

        public string Picture { get; set; }

        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }
    }
}
