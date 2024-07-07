using System.Windows.Input;
using System.Windows.Controls;
using ReManage.Core;
using Npgsql;
using System.Windows;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

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

        private string _adminUserId;
        public string AdminUserId
        {
            get => _adminUserId;
            set
            {
                _adminUserId = value;
                OnPropertyChanged(nameof(AdminUserId));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand TestConnectionCommand { get; }

        public ICommand CreateDatabaseCommand { get; }

        public DatabaseSettingsViewModel()
        {
            LoadCurrentSettings();
            SaveCommand = new RelayCommand(SaveSettings);
            TestConnectionCommand = new RelayCommand(TestConnection);
            CreateDatabaseCommand = new RelayCommand(CreateDatabase);
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

        private async void CreateDatabase(object parameter)
        {
            var result = MessageBox.Show("Вы уверены, что хотите создать новую базу данных? Это действие перезапишет существующую базу данных, если она существует.",
                                         "Подтверждение",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var adminPasswordBox = parameter as PasswordBox;
                var adminPassword = adminPasswordBox?.Password;

                if (string.IsNullOrEmpty(AdminUserId) || string.IsNullOrEmpty(adminPassword))
                {
                    MessageBox.Show("Пожалуйста, введите учетные данные администратора PostgreSQL.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var builderPostgres = new NpgsqlConnectionStringBuilder
                {
                    Host = Server,
                    Port = int.Parse(Port),
                    Database = "postgres", // Подключаемся к системной БД postgres для создания новой БД
                    Username = AdminUserId,
                    Password = adminPassword
                };

                var builderReManage = new NpgsqlConnectionStringBuilder
                {
                    Host = Server,
                    Port = int.Parse(Port),
                    Database = Database,
                    Username = AdminUserId,
                    Password = adminPassword
                };

                try
                {
                    // Создаем новую базу данных
                    using (var connection = new NpgsqlConnection(builderPostgres.ToString()))
                    {
                        await connection.OpenAsync();
                        using (var cmd = new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{Database}\"; CREATE DATABASE \"{Database}\" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_United States.1251'", connection))
                        {
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    // Подключаемся к новой базе данных и выполняем скрипт
                    using (var connection = new NpgsqlConnection(builderReManage.ToString()))
                    {
                        await connection.OpenAsync();
                        string sql = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReManage_DB.sql"));

                        // Извлекаем только валидные SQL команды
                        var validCommands = ExtractValidSqlCommands(sql);

                        foreach (var command in validCommands)
                        {
                            try
                            {
                                using (var cmd = new NpgsqlCommand(command, connection))
                                {
                                    await cmd.ExecuteNonQueryAsync();
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"Ошибка в команде: {command}\n\nОшибка: {ex.Message}");
                            }
                        }
                    }

                    MessageBox.Show("База данных успешно создана и заполнена данными.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании базы данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private List<string> ExtractValidSqlCommands(string sql)
        {
            var commands = new List<string>();
            var currentCommand = new StringBuilder();
            bool inMultilineCommand = false;

            foreach (var line in sql.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string trimmedLine = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("--"))
                    continue;

                if (trimmedLine.StartsWith("CREATE ") || trimmedLine.StartsWith("ALTER ") || trimmedLine.StartsWith("INSERT "))
                {
                    if (currentCommand.Length > 0)
                    {
                        commands.Add(currentCommand.ToString());
                        currentCommand.Clear();
                    }
                    inMultilineCommand = true;
                }

                currentCommand.AppendLine(trimmedLine);

                if (trimmedLine.EndsWith(";"))
                {
                    if (inMultilineCommand || currentCommand.Length > 0)
                    {
                        commands.Add(currentCommand.ToString());
                        currentCommand.Clear();
                        inMultilineCommand = false;
                    }
                }
            }

            if (currentCommand.Length > 0)
            {
                commands.Add(currentCommand.ToString());
            }

            return commands;
        }
    }
}