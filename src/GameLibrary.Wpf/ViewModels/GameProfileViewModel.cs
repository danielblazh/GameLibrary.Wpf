using GameLibrary.Wpf.Models;
using GameLibrary.Wpf.Services;

namespace GameLibrary.Wpf.ViewModels
{
    public class GameProfileViewModel : BaseViewModel
    {
        private readonly MainViewModel _main;
        private readonly Game? _existingGame;

        public GameProfileViewModel(MainViewModel main, Game? game = null)
        {
            _main = main;
            _existingGame = game;

            IsEditing = game != null;
            PageTitle = IsEditing ? TranslationSource.Instance["EditGameTitle"] : TranslationSource.Instance["AddGameTitle"];

            StatusOptions = new List<string> { "Playing", "Queued", "Completed", "Dropped" };
            GenreOptions = new List<string> { "Action", "Adventure", "RPG", "Strategy", "Sports", "Puzzle", "Simulation", "Horror", "FPS", "Racing", "Other" };
            PlatformOptions = new List<string> { "PC", "PlayStation", "Xbox", "Nintendo Switch", "Mobile", "Other" };
            RatingOptions = Enumerable.Range(0, 11).ToList(); // 0-10
            LevelOptions = Enumerable.Range(1, 5).ToList(); // 1-5

            if (game != null)
            {
                Title = game.Title;
                Genre = game.Genre;
                Platform = game.Platform;
                Rating = game.Rating;
                Status = game.Status;
                Level = game.Level;
                HoursPlayed = game.HoursPlayed.ToString("F1");
                ReleaseDate = game.ReleaseDate;
                Notes = game.Notes;
            }
            else
            {
                Status = "Queued";
                Rating = 0;
                Level = 1;
                HoursPlayed = "0";
            }

            SaveCommand = new RelayCommand(DoSave, CanSave);
            CancelCommand = new RelayCommand(() => _main.NavigateToGameLibrary());
        }

        public bool IsEditing { get; }
        public string PageTitle { get; }
        public List<string> StatusOptions { get; }
        public List<string> GenreOptions { get; }
        public List<string> PlatformOptions { get; }
        public List<int> RatingOptions { get; }
        public List<int> LevelOptions { get; }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set { SetProperty(ref _title, value); ValidateTitle(); SaveCommand?.RaiseCanExecuteChanged(); }
        }

        private string _genre = string.Empty;
        public string Genre
        {
            get => _genre;
            set { SetProperty(ref _genre, value); SaveCommand?.RaiseCanExecuteChanged(); }
        }

        private string _platform = string.Empty;
        public string Platform
        {
            get => _platform;
            set { SetProperty(ref _platform, value); SaveCommand?.RaiseCanExecuteChanged(); }
        }

        private int _rating;
        public int Rating
        {
            get => _rating;
            set => SetProperty(ref _rating, value);
        }

        private string _status = "Queued";
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private int _level = 1;
        public int Level
        {
            get => _level;
            set => SetProperty(ref _level, value);
        }

        private string _hoursPlayed = "0";
        public string HoursPlayed
        {
            get => _hoursPlayed;
            set { SetProperty(ref _hoursPlayed, value); ValidateHours(); SaveCommand?.RaiseCanExecuteChanged(); }
        }

        private DateTime? _releaseDate;
        public DateTime? ReleaseDate
        {
            get => _releaseDate;
            set => SetProperty(ref _releaseDate, value);
        }

        private string _notes = string.Empty;
        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        // Validation
        private string _titleError = string.Empty;
        public string TitleError { get => _titleError; set => SetProperty(ref _titleError, value); }

        private string _hoursError = string.Empty;
        public string HoursError { get => _hoursError; set => SetProperty(ref _hoursError, value); }

        private string _errorMessage = string.Empty;
        public string ErrorMessage { get => _errorMessage; set { SetProperty(ref _errorMessage, value); OnPropertyChanged(nameof(HasError)); } }
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void ValidateTitle()
        {
            if (string.IsNullOrWhiteSpace(Title))
                TitleError = TranslationSource.Instance["FieldRequired"];
            else
                TitleError = string.Empty;
        }

        private void ValidateHours()
        {
            if (!double.TryParse(HoursPlayed, out double h) || h < 0)
                HoursError = TranslationSource.Instance["ValueMustBePositive"];
            else
                HoursError = string.Empty;
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Title)
                && !string.IsNullOrWhiteSpace(Genre)
                && !string.IsNullOrWhiteSpace(Platform)
                && double.TryParse(HoursPlayed, out double h) && h >= 0;
        }

        private void DoSave()
        {
            ErrorMessage = string.Empty;
            var userId = _main.Auth.CurrentUser?.Id ?? 0;

            // Check duplicates
            if (_main.Db.GameExistsForUser(Title, userId, _existingGame?.Id))
            {
                ErrorMessage = TranslationSource.Instance["DuplicateGame"];
                return;
            }

            var game = _existingGame ?? new Game();
            game.Title = Title;
            game.Genre = Genre;
            game.Platform = Platform;
            game.Rating = Rating;
            game.Status = Status;
            game.Level = Level;
            game.HoursPlayed = double.Parse(HoursPlayed);
            game.ReleaseDate = ReleaseDate;
            game.Notes = Notes;
            game.UserId = userId;

            if (IsEditing)
                _main.Db.UpdateGame(game);
            else
                _main.Db.AddGame(game);

            _main.NavigateToGameLibrary();
        }
    }
}
