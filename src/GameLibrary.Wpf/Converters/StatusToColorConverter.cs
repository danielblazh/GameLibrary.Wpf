using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GameLibrary.Wpf.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() switch
            {
                "Playing" => new SolidColorBrush(Color.FromRgb(0, 184, 148)),    // ירוק
                "Queued" => new SolidColorBrush(Color.FromRgb(253, 203, 110)),   // כתום
                "Completed" => new SolidColorBrush(Color.FromRgb(9, 132, 227)),  // כחול
                "Dropped" => new SolidColorBrush(Color.FromRgb(214, 48, 49)),    // אדום
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
