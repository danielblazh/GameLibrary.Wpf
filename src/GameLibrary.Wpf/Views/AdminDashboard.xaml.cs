using System.Windows;
using System.Windows.Controls;
using GameLibrary.Wpf.ViewModels;

namespace GameLibrary.Wpf.Views
{
    public partial class AdminDashboard : UserControl
    {
        public AdminDashboard()
        {
            InitializeComponent();
        }

        private void FormPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AdminViewModel vm)
                vm.FormPassword = FormPasswordBox.Password;
        }
    }
}
