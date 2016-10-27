using System;
using System.ComponentModel.DataAnnotations;
using Auction.Presentation.Localization;
using Microsoft.AspNet.Identity;

namespace Auction.Presentation.Models
{
    public class UserViewModel : IUser<string>
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errStringLength")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errStringLength")]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        public string Locale { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        public string TimezoneId { get; set; }

        public string Picture { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errStringLength")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errStringLength")]
        public string LastName { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); } 
        }
    }
}