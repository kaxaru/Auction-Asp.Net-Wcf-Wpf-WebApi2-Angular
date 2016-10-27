using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace WPF.Converters
{
    public class OutdatedTimerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StringBuilder sb = new StringBuilder();
            var countSeconds = long.Parse(value.ToString());
            var days = (countSeconds - (countSeconds % 86400)) / 86400;
            countSeconds -= days * 86400;
            var hours = (countSeconds - (countSeconds % 3600)) / 3600;
            countSeconds -= hours * 3600;
            var minutes = (countSeconds - (countSeconds % 60)) / 60;
            countSeconds -= minutes * 60;
            var seconds = countSeconds;
            sb.Append($"{days} {hours}:{minutes}:{seconds}");
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
