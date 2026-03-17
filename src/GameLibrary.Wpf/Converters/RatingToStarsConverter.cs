using System.Globalization;
using System.Windows.Data;

namespace GameLibrary.Wpf.Converters
{
    public class RatingToStarsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int rating && rating > 0)
            {
                int stars = (int)Math.Ceiling(rating / 2.0);
                return new string('\u2B50', stars) + new string('\u2606', 5 - stars);
            }
            return "\u2606\u2606\u2606\u2606\u2606";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
