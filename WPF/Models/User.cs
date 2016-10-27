using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF.Models
{
    public class User 
    {
        public Guid Id { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string Locale { get; set; }

        public string TimezoneId { get; set; }

        public string Picture { get; set; }
    }
}
