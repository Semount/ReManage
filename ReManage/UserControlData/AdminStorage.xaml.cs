using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using NodaTime;
using NodaTime.Text;
using Npgsql;
using ReManage.Core;
using ReManage.ViewModels;

namespace ReManage.UserControlData
{
    public partial class AdminStorage : UserControl
    {
        private ObservableCollection<RefrigeratorViewModel> refrigeratorProducts = new ObservableCollection<RefrigeratorViewModel>();
        private ObservableCollection<StorageViewModel> storageProducts = new ObservableCollection<StorageViewModel>();

        public AdminStorage()
        {
            InitializeComponent();
            RefrigeratorButton_Click(null, null); // Загрузка продуктов из холодильника по умолчанию
        }

        private void RefrigeratorButton_Click(object sender, RoutedEventArgs e)
        {
            LoadRefrigeratorProducts();
            ProductItemsControl.ItemsSource = refrigeratorProducts;
            ProductItemsControl.ItemTemplate = (DataTemplate)this.Resources["RefrigeratorProductTemplate"];
        }

        private void StorageButton_Click(object sender, RoutedEventArgs e)
        {
            LoadStorageProducts();
            ProductItemsControl.ItemsSource = storageProducts;
            ProductItemsControl.ItemTemplate = (DataTemplate)this.Resources["StorageProductTemplate"];
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            // Открытие окна добавления продукта
            var addProductWindow = new AddProductWindow();
            addProductWindow.ShowDialog();

            // Обновление списка после добавления
            if (addProductWindow.ProductType == "Refrigerator")
            {
                LoadRefrigeratorProducts();
                ProductItemsControl.ItemsSource = refrigeratorProducts;
                ProductItemsControl.ItemTemplate = (DataTemplate)this.Resources["RefrigeratorProductTemplate"];
            }
            else if (addProductWindow.ProductType == "Storage")
            {
                LoadStorageProducts();
                ProductItemsControl.ItemsSource = storageProducts;
                ProductItemsControl.ItemTemplate = (DataTemplate)this.Resources["StorageProductTemplate"];
            }
        }

        private void LoadRefrigeratorProducts()
        {
            refrigeratorProducts.Clear();
            string query = @"
    SELECT id, name, weight, price, amount, date_delivered, 
           expiration_date,
           unfreeze_time
    FROM v_refrigerator_products";

            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new RefrigeratorViewModel
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Weight = reader.GetInt32(2),
                                Price = reader.GetDecimal(3),
                                Amount = reader.GetInt32(4),
                                DateDelivered = reader.GetDateTime(5),
                                ExpirationDate = reader.GetDateTime(6),
                                ///UnfreezeTime = reader.GetTimeSpan(7)
                            };

                            refrigeratorProducts.Add(product);
                        }
                    }
                }
            }
        }

        private void LoadStorageProducts()
        {
            storageProducts.Clear();
            string query = @"
    SELECT id, name, weight, price, amount, date_delivered, 
           expiration_date
    FROM v_storage_products";

            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new StorageViewModel
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Weight = reader.GetInt32(2),
                                Price = reader.GetDecimal(3),
                                Amount = reader.GetInt32(4),
                                DateDelivered = reader.GetDateTime(5),
                                ExpirationDate = reader.GetDateTime(6)
                            };

                            storageProducts.Add(product);
                        }
                    }
                }
            }
        }
    }
}