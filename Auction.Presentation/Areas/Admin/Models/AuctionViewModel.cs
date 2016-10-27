using System.ComponentModel.DataAnnotations;
using Auction.Presentation.Localization;

namespace Auction.Presentation.Areas.Admin.Models
{
    public enum TypeAuctionVM
    {
        Sql = 1,
        Json
    }

    public class AuctionViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errStringLength")]
        [RegularExpression(@"^(\w+)$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errOnlySymbol")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errStringLength")]
        [RegularExpression(@"^(\w+)$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errOnlySymbol")]
        public string Location { get; set; }

        public TypeAuctionVM Type { get; set; }
    }
}