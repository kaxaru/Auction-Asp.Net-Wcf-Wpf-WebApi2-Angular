using System;
using System.ComponentModel.DataAnnotations;
using Auction.Presentation.Localization;

namespace Auction.Presentation.Models
{
    public class ProfileViewModel
    {
        [ScaffoldColumn(false)]
        public Guid Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errStringLength")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errStringLength")]
        public string LastName { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string Locale { get; set; }

        public string TimezoneId { get; set; }

        public string Picture { get; set; }
    }
}