using System.Collections.ObjectModel;
using GameLibrary.Wpf.Services;

namespace GameLibrary.Wpf.ViewModels
{
    public class LeaderboardViewModel : BaseViewModel
    {
        private readonly MainViewModel _main;

        public LeaderboardViewModel(MainViewModel main)
        {
            _main = main;
            BackCommand = new RelayCommand(() => _main.NavigateToDashboard());
            CurrentUsername = _main.Auth.CurrentUser?.Username ?? string.Empty;
            LoadLeaderboard();
        }

        public ObservableCollection<LeaderboardEntry> Entries { get; } = new();
        public string CurrentUsername { get; }

        public RelayCommand BackCommand { get; }

        private void LoadLeaderboard()
        {
            foreach (var entry in _main.Db.GetLeaderboard())
                Entries.Add(entry);
        }
    }
}
