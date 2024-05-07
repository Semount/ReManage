// MainWindow.xaml.cs
using ReManage.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Получаем ссылку на ViewModel
                var viewModel = DataContext as LoginViewModel;

                // Если фокус на поле ввода пароля, выполняем команду для авторизации
                if (Keyboard.FocusedElement == PasswordBox && !string.IsNullOrEmpty(viewModel?.Username) && !string.IsNullOrEmpty(viewModel?.PasswordProvider?.Password))
                {
                    viewModel?.LoginCommand?.Execute(this);
                }
                // Если фокус на поле ввода логина, переместим фокус на поле ввода пароля
                else if (Keyboard.FocusedElement == UsernameBox)
                {
                    PasswordBox.Focus();
                }
                else
                {
                    // Перемещаем фокус на поле ввода логина
                    UsernameBox.Focus();
                }
            }
        }
    }
}