using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace UserService.Model
{
    [DataContract]
    public class User : JsonModel
    {
        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public DateTime RegistrationDate { get; set; }

        [DataMember]
        public string Locale { get; set; }

        [DataMember]
        public string TimezoneId { get; set; }

        [DataMember]
        public string Picture { get; set; }
    }
}