using System;
using System.Globalization;
using System.Windows.Data;
using WPF.Classes;

namespace WPF.Converters
{
    public class PriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var productInfo = value as ProductForBinding;
            if (productInfo.Bid == null)
            {
                productInfo.LastPrice = productInfo.Product.StartPrice;
                return $"NO BIDS... Start Price: {productInfo.Product.StartPrice}";
            }
            else
            {
                productInfo.LastPrice = productInfo.Bid.Price;
                return $"Last Bid: {productInfo.Bid.Price}";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
