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
            Random random = new Random(GetPlatformIndependentHashCode((string)value));

            return new SolidColorBrush(Color.FromRgb((byte)(random.Next(130) + 80), (byte)(random.Next(130) + 80), (byte)(random.Next(130) + 80)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private int GetPlatformIndependentHashCode(string s)
        {
            unchecked
            {
                int hash = 23;
                for (int i = 0; i < s.Length; i++)
                    hash = hash * 31 + s[i];
                return hash;
            }
        }
    }
}
