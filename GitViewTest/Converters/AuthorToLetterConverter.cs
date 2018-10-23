using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace GitViewTest.Converters
{
    public class AuthorToLetterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value).First().ToString().ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
