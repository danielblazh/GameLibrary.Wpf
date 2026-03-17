using System.Windows;
using GameLibrary.Wpf.Services;

namespace GameLibrary.Wpf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly NavigationService _navigation;
        private readonly DatabaseService _db;
        private readonly AuthService _auth;

        public MainViewModel()
        {
            _db = new DatabaseService();
            _auth = new AuthService(_db);
            _navigation = new NavigationService();
            _isDarkMode = true;

            ToggleThemeCommand = new RelayCommand(ToggleTheme);
            LogoutCommand = new RelayCommand(Logout);
            NavigateToDashboardCommand = new RelayCommand(NavigateToDashboard);
            NavigateToLibraryCommand = new RelayCommand(NavigateToGameLibrary);
            NavigateToStatsCommand = new RelayCommand(NavigateToStatistics);
            NavigateToAdminCommand = new RelayCommand(NavigateToAdmin);

            ApplyTheme();
            NavigateToMainPage();
        }

        public NavigationService Navigation => _navigation;
        public DatabaseService Db => _db;
        public AuthService Auth => _auth;

        private bool _isDarkMode;
        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                SetProperty(ref _isDarkMode, value);
                OnPropertyChanged(nameof(ThemeIcon));
            }
        }

        public string ThemeIcon => IsDarkMode ? "\u2600\uFE0F" : "\U0001F319"; // ☀️ / 🌙

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set => SetProperty(ref _isLoggedIn, value);
        }

        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            set => SetProperty(ref _isAdmin, value);
        }

        private string _currentUsername = string.Empty;
        public string CurrentUsername
        {
            get => _currentUsername;
            set => SetProperty(ref _currentUsername, value);
        }

        public RelayCommand ToggleThemeCommand { get; }
        public RelayCommand LogoutCommand { get; }
        public RelayCommand NavigateToDashboardCommand { get; }
        public RelayCommand NavigateToLibraryCommand { get; }
        public RelayCommand NavigateToStatsCommand { get; }
        public RelayCommand NavigateToAdminCommand { get; }

        private void ToggleTheme()
        {
            IsDarkMode = !IsDarkMode;
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            var dict = Application.Current.Resources.MergedDictionaries;
            // Remove old theme (keep Colors and AppStyles)
            for (int i = dict.Count - 1; i >= 0; i--)
            {
                var source = dict[i].Source?.ToString() ?? "";
                if (source.Contains("Theme"))
                    dict.RemoveAt(i);
            }

            dict.Add(new ResourceDictionary
            {
                Source = IsDarkMode
                    ? new Uri("Assets/Styles/DarkTheme.xaml", UriKind.Relative)
                    : new Uri("Assets/Styles/LightTheme.xaml", UriKind.Relative)
            });
        }

        public void NavigateToMainPage()
        {
            IsLoggedIn = false;
            IsAdmin = false;
            CurrentUsername = string.Empty;
            _navigation.NavigateTo(new MainPageViewModel(this));
        }

        public void NavigateToLogin()
        {
            _navigation.NavigateTo(new LoginViewModel(this));
        }

        public void NavigateToRegister()
        {
            _navigation.NavigateTo(new RegisterViewModel(this));
        }

        public void NavigateToDashboard()
        {
            if (_auth.CurrentUser == null) return;
            IsLoggedIn = true;
            IsAdmin = _auth.CurrentUser.Role == "Admin";
            CurrentUsername = _auth.CurrentUser.Username;
            _navigation.NavigateTo(new DashboardViewModel(this));
        }

        public void NavigateToGameLibrary()
        {
            _navigation.NavigateTo(new GameLibraryViewModel(this));
        }

        public void NavigateToGameProfile(Models.Game? game = null)
        {
            _navigation.NavigateTo(new GameProfileViewModel(this, game));
        }

        public void NavigateToStatistics()
        {
            _navigation.NavigateTo(new StatisticsViewModel(this));
        }

        public void NavigateToAdmin()
        {
            _navigation.NavigateTo(new AdminViewModel(this));
        }

        private void Logout()
        {
            _auth.Logout();
            NavigateToMainPage();
        }
    }
}
