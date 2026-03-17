using System.Windows;
using System.Windows.Controls;
using GameLibrary.Wpf.ViewModels;

namespace GameLibrary.Wpf.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
                vm.Password = PasswordBox.Password;
        }

        private void TogglePassword_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
                vm.ShowPassword = !vm.ShowPassword;
        }
    }
}
