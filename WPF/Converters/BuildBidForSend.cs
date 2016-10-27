using System;
using System.Globalization;
using System.Windows.Data;
using WPF.Models;

namespace WPF.Converters
{
    public class BuildBidForSend : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var user = values[2] as User;
            var result = new Bid
            {
                Id = Guid.NewGuid(), DateTime = DateTime.Now, Price = System.Convert.ToInt32(values[0]), ProductId = Guid.Parse(values[1].ToString()),
                UserId = user == null ? Guid.Empty : user.Id
            };

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
