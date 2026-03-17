using System.Text.RegularExpressions;

namespace GameLibrary.Wpf.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly MainViewModel _main;

        public RegisterViewModel(MainViewModel main)
        {
            _main = main;
            RegisterCommand = new RelayCommand(DoRegister, CanRegister);
            BackCommand = new RelayCommand(() => _main.NavigateToLogin());
        }

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                SetProperty(ref _username, value);
                ValidateUsername();
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
                ValidateEmail();
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                ValidatePassword();
                ValidateConfirmPassword();
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }

        private string _confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                SetProperty(ref _confirmPassword, value);
                ValidateConfirmPassword();
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _showPassword;
        public bool ShowPassword
        {
            get => _showPassword;
            set => SetProperty(ref _showPassword, value);
        }

        // Validation errors
        private string _usernameError = string.Empty;
        public string UsernameError { get => _usernameError; set => SetProperty(ref _usernameError, value); }

        private string _emailError = string.Empty;
        public string EmailError { get => _emailError; set => SetProperty(ref _emailError, value); }

        private string _passwordError = string.Empty;
        public string PasswordError { get => _passwordError; set => SetProperty(ref _passwordError, value); }

        private string _confirmPasswordError = string.Empty;
        public string ConfirmPasswordError { get => _confirmPasswordError; set => SetProperty(ref _confirmPasswordError, value); }

        private string _successMessage = string.Empty;
        public string SuccessMessage { get => _successMessage; set { SetProperty(ref _successMessage, value); OnPropertyChanged(nameof(HasSuccess)); } }
        public bool HasSuccess => !string.IsNullOrEmpty(SuccessMessage);

        private string _errorMessage = string.Empty;
        public string ErrorMessage { get => _errorMessage; set { SetProperty(ref _errorMessage, value); OnPropertyChanged(nameof(HasError)); } }
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public RelayCommand RegisterCommand { get; }
        public RelayCommand BackCommand { get; }

        private void ValidateUsername()
        {
            if (string.IsNullOrWhiteSpace(Username))
                UsernameError = "\u26A0 \u05E9\u05D3\u05D4 \u05D6\u05D4 \u05D7\u05D5\u05D1\u05D4"; // שדה זה חובה
            else if (Username.Length < 3)
                UsernameError = "\u26A0 \u05E9\u05DD \u05DE\u05E9\u05EA\u05DE\u05E9 \u05D7\u05D9\u05D9\u05D1 \u05DC\u05D4\u05DB\u05D9\u05DC \u05DC\u05E4\u05D7\u05D5\u05EA 3 \u05EA\u05D5\u05D5\u05D9\u05DD"; // שם משתמש חייב להכיל לפחות 3 תווים
            else if (Username.Contains(' '))
                UsernameError = "\u26A0 \u05E9\u05DD \u05DE\u05E9\u05EA\u05DE\u05E9 \u05DC\u05DC\u05D0 \u05E8\u05D5\u05D5\u05D7\u05D9\u05DD"; // שם משתמש ללא רווחים
            else
                UsernameError = string.Empty;
        }

        private void ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
                EmailError = "\u26A0 \u05E9\u05D3\u05D4 \u05D6\u05D4 \u05D7\u05D5\u05D1\u05D4";
            else if (!EmailRegex().IsMatch(Email))
                EmailError = "\u26A0 \u05DB\u05EA\u05D5\u05D1\u05EA \u05D0\u05D9\u05DE\u05D9\u05D9\u05DC \u05DC\u05D0 \u05EA\u05E7\u05D9\u05E0\u05D4"; // כתובת אימייל לא תקינה
            else
                EmailError = string.Empty;
        }

        private void ValidatePassword()
        {
            if (string.IsNullOrWhiteSpace(Password))
                PasswordError = "\u26A0 \u05E9\u05D3\u05D4 \u05D6\u05D4 \u05D7\u05D5\u05D1\u05D4";
            else if (Password.Length < 6)
                PasswordError = "\u26A0 \u05D4\u05E1\u05D9\u05E1\u05DE\u05D4 \u05D7\u05DC\u05E9\u05D4 \u05DE\u05D3\u05D9"; // הסיסמה חלשה מדי
            else if (!Password.Any(char.IsDigit))
                PasswordError = "\u26A0 \u05D4\u05E1\u05D9\u05E1\u05DE\u05D4 \u05D7\u05DC\u05E9\u05D4 \u05DE\u05D3\u05D9"; // הסיסמה חלשה מדי
            else
                PasswordError = string.Empty;
        }

        private void ValidateConfirmPassword()
        {
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
                ConfirmPasswordError = "\u26A0 \u05E9\u05D3\u05D4 \u05D6\u05D4 \u05D7\u05D5\u05D1\u05D4";
            else if (ConfirmPassword != Password)
                ConfirmPasswordError = "\u26A0 \u05D4\u05E1\u05D9\u05E1\u05DE\u05D0\u05D5\u05EA \u05D0\u05D9\u05E0\u05DF \u05EA\u05D5\u05D0\u05DE\u05D5\u05EA"; // הסיסמאות אינן תואמות
            else
                ConfirmPasswordError = string.Empty;
        }

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(Username) && Username.Length >= 3 && !Username.Contains(' ')
                && !string.IsNullOrWhiteSpace(Email) && EmailRegex().IsMatch(Email)
                && !string.IsNullOrWhiteSpace(Password) && Password.Length >= 6 && Password.Any(char.IsDigit)
                && ConfirmPassword == Password;
        }

        private void DoRegister()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (_main.Auth.Register(Username, Email, Password))
            {
                SuccessMessage = "\u2705 \u05E0\u05E8\u05E9\u05DE\u05EA \u05D1\u05D4\u05E6\u05DC\u05D7\u05D4! \u05DE\u05E2\u05D1\u05D9\u05E8 \u05DC\u05DB\u05E0\u05D9\u05E1\u05D4..."; // ✅ נרשמת בהצלחה!
                _main.NavigateToLogin();
            }
            else
            {
                ErrorMessage = "\u26A0 \u05E9\u05DD \u05DE\u05E9\u05EA\u05DE\u05E9 \u05DB\u05D1\u05E8 \u05E7\u05D9\u05D9\u05DD"; // שם משתמש כבר קיים
            }
        }

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex EmailRegex();
    }
}
