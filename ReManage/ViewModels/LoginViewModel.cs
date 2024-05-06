using System.Windows;
using System.Windows.Input;
using ReManage.Core;
using ReManage.Views;

namespace ReManage.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        private string _password;

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private IPasswordProvider _passwordProvider;
        public IPasswordProvider PasswordProvider
        {
            get { return _passwordProvider; }
            set
            {
                _passwordProvider = value;
                OnPropertyChanged(nameof(PasswordProvider));
            }
        }

        public LoginViewModel()
        {
            PasswordProvider = new PasswordProvider();
        }

        private ICommand _loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                if (_loginCommand == null)
                {
                    _loginCommand = new RelayCommand(LoginAction, CanLogin);
                }
                return _loginCommand;
            }
        }
        private bool CanLogin(object arg)
        {
            // Добавьте здесь логику для проверки возможности входа
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(PasswordProvider?.Password);
        }

        private void LoginAction(object obj)
        {
            // Здесь вы можете добавить код для проверки учетных данных
            // Например:
            if (Username == "admin" && PasswordProvider.Password == "123")
            {
                // Открытие нового окна после успешного входа
                MainMenuWindow mainMenuWindow = new MainMenuWindow();
                mainMenuWindow.Show();

                // Закрытие текущего окна
                var window = obj as Window;
                window?.Close();
            }
            else
            {
                // Выводим сообщение об ошибке при неверных учетных данных
                MessageBox.Show("Неверный логин или пароль.");
            }
        }

        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(CloseAction);
                }
                return _closeCommand;
            }
        }

        private void CloseAction(object obj)
        {
            if (obj is Window window)
            {
                window.Close();
            }
        }
    }
}