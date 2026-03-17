using GameLibrary.Wpf.ViewModels;

namespace GameLibrary.Wpf.Services
{
    public class NavigationService : BaseViewModel
    {
        private BaseViewModel _currentView = null!;
        public BaseViewModel CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public event Action? NavigationChanged;

        public void NavigateTo(BaseViewModel viewModel)
        {
            CurrentView = viewModel;
            NavigationChanged?.Invoke();
        }
    }
}
