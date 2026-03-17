using System.Globalization;
using System.Windows.Data;
using GameLibrary.Wpf.Services;

namespace GameLibrary.Wpf.Converters
{
    public class LevelToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is int level ? level switch
            {
                1 => TranslationSource.Instance["LevelBeginner"],
                2 => TranslationSource.Instance["LevelIntermediate"],
                3 => TranslationSource.Instance["LevelAdvanced"],
                4 => TranslationSource.Instance["LevelExpert"],
                5 => TranslationSource.Instance["LevelMaster"],
                _ => TranslationSource.Instance["LevelUnknown"]
            } : TranslationSource.Instance["LevelUnknown"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
