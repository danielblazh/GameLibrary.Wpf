namespace GameLibrary.Wpf.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private readonly MainViewModel _main;

        public MainPageViewModel(MainViewModel main)
        {
            _main = main;
            LoginCommand = new RelayCommand(() => _main.NavigateToLogin());
            RegisterCommand = new RelayCommand(() => _main.NavigateToRegister());
        }

        public RelayCommand LoginCommand { get; }
        public RelayCommand RegisterCommand { get; }
    }
}
