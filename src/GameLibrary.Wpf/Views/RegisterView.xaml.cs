using System.Windows;
using System.Windows.Controls;
using GameLibrary.Wpf.ViewModels;

namespace GameLibrary.Wpf.Views
{
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
                vm.Password = PasswordBox.Password;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
                vm.ConfirmPassword = ConfirmPasswordBox.Password;
        }

        private void TogglePassword_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
                vm.ShowPassword = !vm.ShowPassword;
        }
    }
}
