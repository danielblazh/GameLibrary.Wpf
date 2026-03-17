using System.Collections.ObjectModel;
using GameLibrary.Wpf.Models;
using GameLibrary.Wpf.Services;

namespace GameLibrary.Wpf.ViewModels
{
    public class GameLibraryViewModel : BaseViewModel
    {
        private readonly MainViewModel _main;
        private List<Game> _allGames = new();

        public GameLibraryViewModel(MainViewModel main)
        {
            _main = main;

            Statuses = new List<string> { TranslationSource.Instance["All"], "Playing", "Queued", "Completed", "Dropped" }; // הכל
            Genres = new List<string> { TranslationSource.Instance["All"] }; // הכל
            SelectedStatus = Statuses[0];
            SelectedGenre = Genres[0];

            AddGameCommand = new RelayCommand(() => _main.NavigateToGameProfile());
            EditGameCommand = new RelayCommand<Game>(g => { if (g != null) _main.NavigateToGameProfile(g); });
            DeleteGameCommand = new RelayCommand<Game>(DoDeleteGame);
            AchievementsCommand = new RelayCommand<Game>(g => { if (g != null) _main.NavigateToAchievements(g.Id, g.Title); });
            BackCommand = new RelayCommand(() => _main.NavigateToDashboard());

            LoadGames();
        }

        public ObservableCollection<Game> Games { get; } = new();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                FilterGames();
            }
        }

        private string _selectedStatus = string.Empty;
        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                SetProperty(ref _selectedStatus, value);
                FilterGames();
            }
        }

        private string _selectedGenre = string.Empty;
        public string SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                SetProperty(ref _selectedGenre, value);
                FilterGames();
            }
        }

        public List<string> Statuses { get; }
        public List<string> Genres { get; set; }

        private int _resultsCount;
        public int ResultsCount
        {
            get => _resultsCount;
            set
            {
                SetProperty(ref _resultsCount, value);
                OnPropertyChanged(nameof(GamesFoundText));
            }
        }

        public string GamesFoundText => string.Format(TranslationSource.Instance["GamesFound"], ResultsCount);

        public RelayCommand AddGameCommand { get; }
        public RelayCommand<Game> EditGameCommand { get; }
        public RelayCommand<Game> DeleteGameCommand { get; }
        public RelayCommand<Game> AchievementsCommand { get; }
        public RelayCommand BackCommand { get; }

        private void LoadGames()
        {
            var userId = _main.Auth.CurrentUser?.Id ?? 0;
            _allGames = _main.Db.GetGamesByUser(userId);

            // Build genre list
            var genres = _allGames.Select(g => g.Genre).Distinct().OrderBy(g => g).ToList();
            genres.Insert(0, TranslationSource.Instance["All"]); // הכל
            Genres = genres;
            OnPropertyChanged(nameof(Genres));

            FilterGames();
        }

        private void FilterGames()
        {
            Games.Clear();
            var filtered = _allGames.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
                filtered = filtered.Where(g => g.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            if (SelectedStatus != TranslationSource.Instance["All"] && !string.IsNullOrEmpty(SelectedStatus)) // הכל
                filtered = filtered.Where(g => g.Status == SelectedStatus);

            if (SelectedGenre != TranslationSource.Instance["All"] && !string.IsNullOrEmpty(SelectedGenre)) // הכל
                filtered = filtered.Where(g => g.Genre == SelectedGenre);

            foreach (var game in filtered)
                Games.Add(game);

            ResultsCount = Games.Count;
        }

        private void DoDeleteGame(Game? game)
        {
            if (game == null) return;
            var result = System.Windows.MessageBox.Show(
                string.Format(TranslationSource.Instance["ConfirmDeleteGame"], game.Title),
                TranslationSource.Instance["ConfirmDelete"],
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                _main.Db.DeleteGame(game.Id);
                LoadGames();
            }
        }
    }
}
