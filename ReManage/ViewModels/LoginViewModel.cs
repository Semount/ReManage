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
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(PasswordProvider?.Password);
        }

        private bool _showErrorMessage;
        public bool ShowErrorMessage
        {
            get { return _showErrorMessage; }
            set
            {
                _showErrorMessage = value;
                OnPropertyChanged(nameof(ShowErrorMessage));
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        private void LoginAction(object obj)
        {
            string enteredLogin = Username;
            string enteredPassword = PasswordProvider.Password;

            using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                string query = "SELECT role_id, name, surname FROM employees WHERE login = @Login AND password = @Password";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Login", enteredLogin);
                    command.Parameters.AddWithValue("@Password", enteredPassword);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int employeeId = reader.GetInt32(0);
                            int roleId = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            string surname = reader.GetString(2);

                            switch (roleId)
                            {
                                case 1: // Официант
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        WaiterWindow waiterWindow = new WaiterWindow(employeeId, name, surname);
                                        waiterWindow.Show();
                                    });
                                    break;
                                case 2: // Повар
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        ChefWindow chefWindow = new ChefWindow(name, surname);
                                        chefWindow.Show();
                                    });
                                    break;
                                case 3: // Администратор
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        AdminWindow adminWindow = new AdminWindow(name, surname);
                                        adminWindow.Show();
                                    });
                                    break;
                                default:
                                    // Неизвестная роль
                                    ShowErrorMessage = true;
                                    ErrorMessage = "Неизвестная роль. Доложите администратору.";
                                    break;
                            }
                            // Закрываем текущее окно входа
                            if (obj is Window window)
                            {
                                window.Close();
                            }
                        }
                        else
                        {
                            // Неверный логин или пароль
                            ShowErrorMessage = true;
                            ErrorMessage = "Неверный логин или пароль.";
                        }
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