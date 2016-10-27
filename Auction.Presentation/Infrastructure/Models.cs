using System.Collections.Generic;
using Auction.DataAccess.Models;

namespace Auction.Presentation.Infrastructure
{
    public class Models
    {
        private readonly Category _categoryModel;
        private readonly Product _productModel;
        private readonly Bid _bidModel;

        public Models()
        {
            _categoryModel = new Category();
            _productModel = new Product();
            _bidModel = new Bid();
        }

        public List<string> GetModel()
        {
            var listModelName = new List<string>()
            {
                _categoryModel.ToString(),
                _productModel.ToString(),
                _bidModel.ToString()
            };
            return listModelName;
        }
    }
}