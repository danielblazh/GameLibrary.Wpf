using GameLibrary.Wpf.Services;

namespace GameLibrary.Wpf.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly MainViewModel _main;

        public LoginViewModel(MainViewModel main)
        {
            _main = main;
            LoginCommand = new RelayCommand(DoLogin,
                () => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password));
            BackCommand = new RelayCommand(() => _main.NavigateToMainPage());
            GoToRegisterCommand = new RelayCommand(() => _main.NavigateToRegister());
        }

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                SetProperty(ref _username, value);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                OnPropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        private bool _showPassword;
        public bool ShowPassword
        {
            get => _showPassword;
            set => SetProperty(ref _showPassword, value);
        }

        public RelayCommand LoginCommand { get; }
        public RelayCommand BackCommand { get; }
        public RelayCommand GoToRegisterCommand { get; }

        private void DoLogin()
        {
            var user = _main.Auth.Login(Username, Password);
            if (user != null)
            {
                ErrorMessage = string.Empty;
                _main.NavigateToDashboard();
            }
            else
            {
                ErrorMessage = TranslationSource.Instance["LoginError"];
            }
        }
    }
}
