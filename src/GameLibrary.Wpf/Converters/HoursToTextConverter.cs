using System.Globalization;
using System.Windows.Data;

namespace GameLibrary.Wpf.Converters
{
    public class HoursToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double hours)
            {
                if (hours < 1) return $"{(int)(hours * 60)} דקות";
                return $"{hours:F1} שעות";
            }
            return "0 שעות";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
