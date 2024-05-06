using System;
using System.Windows;
using System.Windows.Input;
using Npgsql;
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
            string enteredLogin = Username;
            string enteredPassword = PasswordProvider.Password;

            using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                string query = "SELECT role_id FROM employees WHERE login = @Login AND password = @Password";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Login", enteredLogin);
                    command.Parameters.AddWithValue("@Password", enteredPassword);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        int roleId = Convert.ToInt32(result);
                        switch (roleId)
                        {
                            case 1: // Официант
                                // Открыть окно для официанта
                                break;
                            case 2: // Повар
                                // Открыть окно для повара
                                break;
                            case 3: // Администратор
                                // Открыть окно для администратора
                                break;
                            default:
                                // Неизвестная роль
                                break;
                        }
                    }
                    else
                    {
                        // Неверный логин или пароль
                        MessageBox.Show("Неверный логин или пароль.");
                    }
                }
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