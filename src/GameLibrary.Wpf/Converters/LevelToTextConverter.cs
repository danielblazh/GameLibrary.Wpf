using System.Globalization;
using System.Windows.Data;

namespace GameLibrary.Wpf.Converters
{
    public class LevelToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is int level ? level switch
            {
                1 => "\u05DE\u05EA\u05D7\u05D9\u05DC",   // מתחיל (Beginner)
                2 => "\u05D1\u05D9\u05E0\u05D5\u05E0\u05D9",   // בינוני (Intermediate)
                3 => "\u05DE\u05EA\u05E7\u05D3\u05DD",   // מתקדם (Advanced)
                4 => "\u05DE\u05D5\u05DE\u05D7\u05D4",   // מומחה (Expert)
                5 => "\u05DE\u05D0\u05E1\u05D8\u05E8",   // מאסטר (Master)
                _ => "\u05DC\u05D0 \u05D9\u05D3\u05D5\u05E2"  // לא ידוע
            } : "\u05DC\u05D0 \u05D9\u05D3\u05D5\u05E2";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
