using System.ComponentModel;
using System.Globalization;
using System.Resources;
using GameLibrary.Wpf.Resources;

namespace GameLibrary.Wpf.Services
{
    public class TranslationSource : INotifyPropertyChanged
    {
        public static TranslationSource Instance { get; } = new();

        private readonly ResourceManager _resourceManager = Strings.ResourceManager;
        private CultureInfo _currentCulture = CultureInfo.InvariantCulture; // English default

        public string this[string key]
        {
            get
            {
                var value = _resourceManager.GetString(key, _currentCulture);
                return value ?? $"[{key}]";
            }
        }

        public CultureInfo CurrentCulture
        {
            get => _currentCulture;
            set
            {
                if (_currentCulture == value) return;
                _currentCulture = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCulture)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRtl)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FlowDirection)));
            }
        }

        public bool IsRtl => _currentCulture.TextInfo.IsRightToLeft;

        public System.Windows.FlowDirection FlowDirection =>
            IsRtl ? System.Windows.FlowDirection.RightToLeft : System.Windows.FlowDirection.LeftToRight;

        public void ToggleLanguage()
        {
            CurrentCulture = _currentCulture.Name == "he"
                ? CultureInfo.InvariantCulture
                : new CultureInfo("he");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
