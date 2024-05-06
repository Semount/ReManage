// MainWindow.xaml.cs
using ReManage.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ReManage
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.PasswordProvider.Password = ((PasswordBox)sender).Password;
            }
        }

        private void CloseCommand(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}