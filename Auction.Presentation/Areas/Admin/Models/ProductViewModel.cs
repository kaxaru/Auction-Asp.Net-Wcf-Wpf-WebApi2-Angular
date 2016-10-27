using System;
using System.ComponentModel.DataAnnotations;
using Auction.BussinessLogic.Models;
using Auction.Presentation.Localization;

namespace Auction.Presentation.Models
{
    public class ProductViewModel
    {
        [ScaffoldColumn(false)]
        public Guid Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errStringLength")]
        public string Name { get; set; }

        [StringLength(200, MinimumLength = 10, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errStringLength")]
        public string Description { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        public TimeSpan Duration { get; set; }

        public State State { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRequired")]
        [Range(typeof(int), "1", "1000000000", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errRange")]
        [RegularExpression(@"^\d{0,10}$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "errPrice")]
        public int StartPrice { get; set; }

        public Guid CategoryID { get; set; }

        public string Picture { get; set; }

        public Guid UserId { get; set; }
    }
}