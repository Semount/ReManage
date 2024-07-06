using System.Windows.Input;
using System.Windows.Controls;
using ReManage.Core;
using Npgsql;
using System.Windows;
using System.Threading.Tasks;

namespace ReManage.ViewModels
{
    public class DatabaseSettingsViewModel : ViewModelBase
    {
        private string _server;
        public string Server
        {
            get => _server;
            set
            {
                _server = value;
                OnPropertyChanged(nameof(Server));
            }
        }

        private string _port;
        public string Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        private string _database;
        public string Database
        {
            get => _database;
            set
            {
                _database = value;
                OnPropertyChanged(nameof(Database));
            }
        }

        private string _userId;
        public string UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged(nameof(UserId));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand TestConnectionCommand { get; }

        public DatabaseSettingsViewModel()
        {
            LoadCurrentSettings();
            SaveCommand = new RelayCommand(SaveSettings);
            TestConnectionCommand = new RelayCommand(TestConnection);
        }

        private void LoadCurrentSettings()
        {
            var currentConnectionString = DatabaseConnection.GetConnectionString();
            if (!string.IsNullOrEmpty(currentConnectionString))
            {
                var builder = new NpgsqlConnectionStringBuilder(currentConnectionString);
                Server = builder.Host;
                Port = builder.Port.ToString();
                Database = builder.Database;
                UserId = builder.Username;
            }
        }

        private async void SaveSettings(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox?.Password;

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = Server,
                Port = int.Parse(Port),
                Database = Database,
                Username = UserId,
                Password = password,
                Pooling = true,
                MinPoolSize = 1,
                MaxPoolSize = 20,
                Timeout = 300
            };

            var newConnectionString = builder.ToString();

            if (await TestConnectionAsync(newConnectionString))
            {
                DatabaseConnection.SetConnectionString(newConnectionString);
                MessageBox.Show("Настройки успешно сохранены и подключение проверено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Не удалось подключиться к базе данных. Пожалуйста, проверьте настройки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void TestConnection(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox?.Password;

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = Server,
                Port = int.Parse(Port),
                Database = Database,
                Username = UserId,
                Password = password,
                Timeout = 10
            };

            var result = await TestConnectionAsync(builder.ToString());
            MessageBox.Show(result ? "Подключение успешно!" : "Не удалось подключиться. Проверьте настройки.",
                            result ? "Успех" : "Ошибка",
                            MessageBoxButton.OK,
                            result ? MessageBoxImage.Information : MessageBoxImage.Error);
        }

        private async Task<bool> TestConnectionAsync(string connectionString)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}