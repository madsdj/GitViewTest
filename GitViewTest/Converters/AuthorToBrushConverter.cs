using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GitViewTest.Converters
{
    public class AuthorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Random random = new Random(((string)value).GetHashCode());

            return new SolidColorBrush(Color.FromRgb((byte)(random.Next(120) + 80), (byte)(random.Next(120) + 80), (byte)(random.Next(120) + 80)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
