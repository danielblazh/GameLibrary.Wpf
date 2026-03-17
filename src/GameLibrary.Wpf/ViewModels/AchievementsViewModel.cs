using System.Collections.ObjectModel;
using GameLibrary.Wpf.Models;
using GameLibrary.Wpf.Services;

namespace GameLibrary.Wpf.ViewModels
{
    public class AchievementsViewModel : BaseViewModel
    {
        private readonly MainViewModel _main;
        private readonly int _gameId;
        private readonly string _gameTitle;

        public AchievementsViewModel(MainViewModel main, int gameId, string gameTitle)
        {
            _main = main;
            _gameId = gameId;
            _gameTitle = gameTitle;

            PageTitle = string.Format(TranslationSource.Instance["AchievementsFor"], gameTitle);

            AddCommand = new RelayCommand(DoAdd, () => !string.IsNullOrWhiteSpace(NewTitle));
            ToggleUnlockCommand = new RelayCommand<Achievement>(DoToggle);
            DeleteCommand = new RelayCommand<Achievement>(DoDelete);
            BackCommand = new RelayCommand(() => _main.NavigateToGameLibrary());

            LoadAchievements();
        }

        public string PageTitle { get; }
        public ObservableCollection<Achievement> Achievements { get; } = new();

        private string _newTitle = string.Empty;
        public string NewTitle
        {
            get => _newTitle;
            set { SetProperty(ref _newTitle, value); AddCommand.RaiseCanExecuteChanged(); }
        }

        private string _newDescription = string.Empty;
        public string NewDescription
        {
            get => _newDescription;
            set => SetProperty(ref _newDescription, value);
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand<Achievement> ToggleUnlockCommand { get; }
        public RelayCommand<Achievement> DeleteCommand { get; }
        public RelayCommand BackCommand { get; }

        private void LoadAchievements()
        {
            Achievements.Clear();
            foreach (var a in _main.Db.GetAchievementsByGame(_gameId))
                Achievements.Add(a);
        }

        private void DoAdd()
        {
            var a = new Achievement
            {
                GameId = _gameId,
                UserId = _main.Auth.CurrentUser?.Id ?? 0,
                Title = NewTitle,
                Description = NewDescription
            };
            _main.Db.AddAchievement(a);
            NewTitle = string.Empty;
            NewDescription = string.Empty;
            LoadAchievements();
        }

        private void DoToggle(Achievement? a)
        {
            if (a == null) return;
            a.IsUnlocked = !a.IsUnlocked;
            a.UnlockedDate = a.IsUnlocked ? DateTime.Now : null;
            _main.Db.UpdateAchievement(a);
            LoadAchievements();
        }

        private void DoDelete(Achievement? a)
        {
            if (a == null) return;
            var result = System.Windows.MessageBox.Show(
                string.Format(TranslationSource.Instance["ConfirmDeleteAchievement"], a.Title),
                TranslationSource.Instance["ConfirmDelete"],
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);
            if (result == System.Windows.MessageBoxResult.Yes)
            {
                _main.Db.DeleteAchievement(a.Id);
                LoadAchievements();
            }
        }
    }
}
