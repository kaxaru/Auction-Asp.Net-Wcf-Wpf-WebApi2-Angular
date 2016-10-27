using System;
using System.ComponentModel.DataAnnotations;
using Auction.Presentation.Localization;

namespace Auction.Presentation.Models
{
    public class RegistrationViewModel
    {
        [ScaffoldColumn(false)]
        public Guid Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errStringLength")]
        public string Login { get; set; }
       
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errStringLength")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errStringLength")]
        [Compare("Password", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errCompare")]
        [DataType(DataType.Password)]
        public string DPassword { get; set; }
       
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