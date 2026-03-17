namespace GameLibrary.Wpf.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly MainViewModel _main;

        public DashboardViewModel(MainViewModel main)
        {
            _main = main;

            var user = _main.Auth.CurrentUser;
            if (user != null)
            {
                var hour = DateTime.Now.Hour;
                string greeting = hour switch
                {
                    < 12 => "\u05D1\u05D5\u05E7\u05E8 \u05D8\u05D5\u05D1", // בוקר טוב
                    < 17 => "\u05E6\u05D4\u05E8\u05D9\u05D9\u05DD \u05D8\u05D5\u05D1\u05D9\u05DD", // צהריים טובים
                    _ => "\u05E2\u05E8\u05D1 \u05D8\u05D5\u05D1"  // ערב טוב
                };
                WelcomeMessage = $"{greeting}, {user.Username}!";

                var games = _main.Db.GetGamesByUser(user.Id);
                TotalGames = games.Count;
                PlayingCount = games.Count(g => g.Status == "Playing");
                CompletedCount = games.Count(g => g.Status == "Completed");
                TotalHours = games.Sum(g => g.HoursPlayed);
            }

            GameLibraryCommand = new RelayCommand(() => _main.NavigateToGameLibrary());
            StatisticsCommand = new RelayCommand(() => _main.NavigateToStatistics());
            AdminCommand = new RelayCommand(() => _main.NavigateToAdmin());
        }

        private string _welcomeMessage = string.Empty;
        public string WelcomeMessage { get => _welcomeMessage; set => SetProperty(ref _welcomeMessage, value); }

        private int _totalGames;
        public int TotalGames { get => _totalGames; set => SetProperty(ref _totalGames, value); }

        private int _playingCount;
        public int PlayingCount { get => _playingCount; set => SetProperty(ref _playingCount, value); }

        private int _completedCount;
        public int CompletedCount { get => _completedCount; set => SetProperty(ref _completedCount, value); }

        private double _totalHours;
        public double TotalHours { get => _totalHours; set => SetProperty(ref _totalHours, value); }

        public bool IsAdmin => _main.Auth.CurrentUser?.Role == "Admin";

        public RelayCommand GameLibraryCommand { get; }
        public RelayCommand StatisticsCommand { get; }
        public RelayCommand AdminCommand { get; }
    }
}
