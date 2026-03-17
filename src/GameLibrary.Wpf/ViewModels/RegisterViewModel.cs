using System.Text.RegularExpressions;
using GameLibrary.Wpf.Services;

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
                UsernameError = TranslationSource.Instance["FieldRequired"];
            else if (Username.Length < 3)
                UsernameError = TranslationSource.Instance["UsernameMinLength"];
            else if (Username.Contains(' '))
                UsernameError = TranslationSource.Instance["UsernameNoSpaces"];
            else
                UsernameError = string.Empty;
        }

        private void ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
                EmailError = TranslationSource.Instance["FieldRequired"];
            else if (!EmailRegex().IsMatch(Email))
                EmailError = TranslationSource.Instance["InvalidEmail"];
            else
                EmailError = string.Empty;
        }

        private void ValidatePassword()
        {
            if (string.IsNullOrWhiteSpace(Password))
                PasswordError = TranslationSource.Instance["FieldRequired"];
            else if (Password.Length < 6)
                PasswordError = TranslationSource.Instance["WeakPassword"];
            else if (!Password.Any(char.IsDigit))
                PasswordError = TranslationSource.Instance["WeakPassword"];
            else
                PasswordError = string.Empty;
        }

        private void ValidateConfirmPassword()
        {
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
                ConfirmPasswordError = TranslationSource.Instance["FieldRequired"];
            else if (ConfirmPassword != Password)
                ConfirmPasswordError = TranslationSource.Instance["PasswordMismatch"];
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
                SuccessMessage = TranslationSource.Instance["RegisterSuccess"];
                _main.NavigateToLogin();
            }
            else
            {
                ErrorMessage = TranslationSource.Instance["UsernameExists"];
            }
        }

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial Regex EmailRegex();
    }
}
