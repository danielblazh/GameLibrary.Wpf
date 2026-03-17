using System.Collections.ObjectModel;

namespace GameLibrary.Wpf.ViewModels
{
    public class StatItem
    {
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class StatisticsViewModel : BaseViewModel
    {
        private readonly MainViewModel _main;

        public StatisticsViewModel(MainViewModel main)
        {
            _main = main;
            BackCommand = new RelayCommand(() => _main.NavigateToDashboard());

            LoadStatistics();
        }

        public int TotalGames { get; private set; }
        public double TotalHours { get; private set; }
        public double AverageRating { get; private set; }
        public int TotalAchievements { get; private set; }
        public int UnlockedAchievements { get; private set; }
        public ObservableCollection<StatItem> StatusStats { get; } = new();
        public ObservableCollection<StatItem> GenreStats { get; } = new();

        public RelayCommand BackCommand { get; }

        private void LoadStatistics()
        {
            var userId = _main.Auth.CurrentUser?.Id ?? 0;

            TotalGames = _main.Db.GetTotalGamesCount(userId);
            TotalHours = _main.Db.GetTotalHoursPlayed(userId);
            AverageRating = _main.Db.GetAverageRating(userId);

            TotalAchievements = _main.Db.GetTotalAchievementsCount(userId);
            UnlockedAchievements = _main.Db.GetUnlockedAchievementsCount(userId);

            OnPropertyChanged(nameof(TotalGames));
            OnPropertyChanged(nameof(TotalHours));
            OnPropertyChanged(nameof(AverageRating));
            OnPropertyChanged(nameof(TotalAchievements));
            OnPropertyChanged(nameof(UnlockedAchievements));

            var byStatus = _main.Db.GetGamesByStatus(userId);
            foreach (var kvp in byStatus)
            {
                StatusStats.Add(new StatItem
                {
                    Label = kvp.Key,
                    Count = kvp.Value,
                    Percentage = TotalGames > 0 ? kvp.Value * 100.0 / TotalGames : 0
                });
            }

            var byGenre = _main.Db.GetGamesByGenre(userId);
            foreach (var kvp in byGenre.OrderByDescending(x => x.Value))
            {
                GenreStats.Add(new StatItem
                {
                    Label = kvp.Key,
                    Count = kvp.Value,
                    Percentage = TotalGames > 0 ? kvp.Value * 100.0 / TotalGames : 0
                });
            }
        }
    }
}
