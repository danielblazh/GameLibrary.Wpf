using System.Globalization;
using System.Windows.Data;

namespace GameLibrary.Wpf.Converters
{
    public class StatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() switch
            {
                "Playing" => "\U0001F3AE",    // 🎮
                "Queued" => "\u23F3",          // ⏳
                "Completed" => "\u2705",       // ✅
                "Dropped" => "\U0001F4A4",     // 💤
                _ => "\u2753"                  // ❓
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
