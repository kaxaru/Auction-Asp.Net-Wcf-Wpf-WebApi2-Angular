using System;
using System.ComponentModel.DataAnnotations;
using Auction.Presentation.Localization;

namespace Auction.Presentation.Models
{
    public class ChangePasswordModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errStringLength")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errStringLength")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errStringLength")]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errCompare")]
        [DataType(DataType.Password)]
        public string RepeatNewPassword { get; set; }
    }
}