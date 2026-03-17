using System.Windows;
using GameLibrary.Wpf.ViewModels;

namespace GameLibrary.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
