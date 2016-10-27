using System;
using System.ComponentModel.DataAnnotations;
using Auction.Presentation.Localization;

namespace Auction.Presentation.Models
{
    public class CategoryViewModel
    {
        [ScaffoldColumn(false)]
        public Guid Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errStringLength")]
        public string Name { get; set; }

        [StringLength(200, MinimumLength = 10, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errStringLength")]
        public string Description { get; set; }
    }
}