using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WPF.Classes;

namespace WPF.Converters
{
    public class NextPriceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var productInfo = values[0] as ProductForBinding;
            var bidOffset = int.Parse(values[1].ToString());

            productInfo.NextPrice = productInfo.Bid == null ? productInfo.Product.StartPrice + bidOffset : productInfo.Bid.Price + bidOffset;
            return $"Next bid: {productInfo.NextPrice} $";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
