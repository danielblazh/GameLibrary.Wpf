using System.Collections.ObjectModel;
using GameLibrary.Wpf.Models;

namespace GameLibrary.Wpf.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        private readonly MainViewModel _main;
        private List<User> _allUsers = new();

        public AdminViewModel(MainViewModel main)
        {
            _main = main;

            Roles = new List<string> { "\u05D4\u05DB\u05DC", "Admin", "User", "Guest" }; // הכל
            SelectedRole = Roles[0];

            SaveUserCommand = new RelayCommand(DoSaveUser, CanSaveUser);
            DeleteUserCommand = new RelayCommand<User>(DoDeleteUser);
            EditUserCommand = new RelayCommand<User>(DoEditUser);
            CancelEditCommand = new RelayCommand(ClearForm);
            BackCommand = new RelayCommand(() => _main.NavigateToDashboard());

            LoadUsers();
        }

        public ObservableCollection<User> Users { get; } = new();
        public List<string> Roles { get; }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); FilterUsers(); }
        }

        private string _selectedRole = string.Empty;
        public string SelectedRole
        {
            get => _selectedRole;
            set { SetProperty(ref _selectedRole, value); FilterUsers(); }
        }

        // Form fields for Add/Edit
        private bool _isEditingUser;
        public bool IsEditingUser { get => _isEditingUser; set => SetProperty(ref _isEditingUser, value); }

        private int _editUserId;

        private string _formUsername = string.Empty;
        public string FormUsername
        {
            get => _formUsername;
            set { SetProperty(ref _formUsername, value); SaveUserCommand.RaiseCanExecuteChanged(); }
        }

        private string _formEmail = string.Empty;
        public string FormEmail
        {
            get => _formEmail;
            set { SetProperty(ref _formEmail, value); SaveUserCommand.RaiseCanExecuteChanged(); }
        }

        private string _formRole = "User";
        public string FormRole
        {
            get => _formRole;
            set { SetProperty(ref _formRole, value); SaveUserCommand.RaiseCanExecuteChanged(); }
        }

        private string _formPassword = string.Empty;
        public string FormPassword
        {
            get => _formPassword;
            set { SetProperty(ref _formPassword, value); SaveUserCommand.RaiseCanExecuteChanged(); }
        }

        public string FormTitle => IsEditingUser ? "\u05E2\u05E8\u05D9\u05DB\u05EA \u05DE\u05E9\u05EA\u05DE\u05E9" : "\u05D4\u05D5\u05E1\u05E4\u05EA \u05DE\u05E9\u05EA\u05DE\u05E9"; // עריכת משתמש / הוספת משתמש

        public RelayCommand SaveUserCommand { get; }
        public RelayCommand<User> DeleteUserCommand { get; }
        public RelayCommand<User> EditUserCommand { get; }
        public RelayCommand CancelEditCommand { get; }
        public RelayCommand BackCommand { get; }

        private void LoadUsers()
        {
            _allUsers = _main.Db.GetAllUsers();
            FilterUsers();
        }

        private void FilterUsers()
        {
            Users.Clear();
            var filtered = _allUsers.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
                filtered = filtered.Where(u =>
                    u.Username.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            if (SelectedRole != "\u05D4\u05DB\u05DC" && !string.IsNullOrEmpty(SelectedRole)) // הכל
                filtered = filtered.Where(u => u.Role == SelectedRole);

            foreach (var user in filtered)
                Users.Add(user);
        }

        private void DoEditUser(User? user)
        {
            if (user == null) return;
            IsEditingUser = true;
            _editUserId = user.Id;
            FormUsername = user.Username;
            FormEmail = user.Email;
            FormRole = user.Role;
            FormPassword = string.Empty;
            OnPropertyChanged(nameof(FormTitle));
        }

        private bool CanSaveUser()
        {
            return !string.IsNullOrWhiteSpace(FormUsername) && FormUsername.Length >= 3
                && !string.IsNullOrWhiteSpace(FormEmail)
                && !string.IsNullOrWhiteSpace(FormRole)
                && (IsEditingUser || (!string.IsNullOrWhiteSpace(FormPassword) && FormPassword.Length >= 6));
        }

        private void DoSaveUser()
        {
            if (IsEditingUser)
            {
                var user = new User
                {
                    Id = _editUserId,
                    Username = FormUsername,
                    Email = FormEmail,
                    Role = FormRole
                };
                _main.Db.UpdateUser(user);
            }
            else
            {
                _main.Auth.Register(FormUsername, FormEmail, FormPassword, FormRole);
            }

            ClearForm();
            LoadUsers();
        }

        private void DoDeleteUser(User? user)
        {
            if (user == null) return;
            var result = System.Windows.MessageBox.Show(
                $"\u05D4\u05D0\u05DD \u05DC\u05DE\u05D7\u05D5\u05E7 \u05D0\u05EA \u05D4\u05DE\u05E9\u05EA\u05DE\u05E9 \"{user.Username}\"?", // האם למחוק את המשתמש
                "\u05D0\u05D9\u05E9\u05D5\u05E8 \u05DE\u05D7\u05D9\u05E7\u05D4", // אישור מחיקה
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                _main.Db.DeleteUser(user.Id);
                LoadUsers();
            }
        }

        private void ClearForm()
        {
            IsEditingUser = false;
            _editUserId = 0;
            FormUsername = string.Empty;
            FormEmail = string.Empty;
            FormRole = "User";
            FormPassword = string.Empty;
            OnPropertyChanged(nameof(FormTitle));
        }
    }
}
