using System.Collections.Generic;

namespace Auction.Presentation.Models
{
    public class CatalogViewModel
    {
        public IEnumerable<CategoryViewModel> CategoriesVM { get; set; }

        public IEnumerable<ProductViewModel> ProductVM { get; set; }
    }
}