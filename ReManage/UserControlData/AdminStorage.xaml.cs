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
        SELECT r.id, p.name, p.weight, p.price, r.amount, r.date_delivered, 
               r.shelf_life, r.unfreeze_time, r.expiry_date
        FROM refrigerator r
        INNER JOIN products p ON r.product_id = p.id
        ORDER BY r.date_delivered";

            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var shelfLifeInterval = reader.GetFieldValue<NpgsqlTypes.NpgsqlInterval>(6);
                            var shelfLifePeriod = new PeriodBuilder { Months = shelfLifeInterval.Months, Days = shelfLifeInterval.Days }.Build();

                            var product = new RefrigeratorViewModel
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Weight = reader.GetInt32(2),
                                Price = reader.GetDecimal(3),
                                Amount = reader.GetInt32(4),
                                DateDelivered = reader.GetDateTime(5),
                                ShelfLife = shelfLifePeriod,
                                UnfreezeTime = reader.IsDBNull(7) ? TimeSpan.Zero : reader.GetTimeSpan(7),
                                ExpiryDate = reader.GetDateTime(8)
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
        SELECT s.id, p.name, p.weight, p.price, s.amount, s.date_delivered, 
               s.shelf_life, s.expiry_date
        FROM storage s
        INNER JOIN products p ON s.product_id = p.id
        ORDER BY date_delivered";

            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var shelfLifeInterval = reader.GetFieldValue<NpgsqlTypes.NpgsqlInterval>(6);
                            var shelfLifePeriod = new PeriodBuilder { Months = shelfLifeInterval.Months, Days = shelfLifeInterval.Days }.Build();
                            var product = new StorageViewModel
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Weight = reader.GetInt32(2),
                                Price = reader.GetDecimal(3),
                                Amount = reader.GetInt32(4),
                                DateDelivered = reader.GetDateTime(5),
                                ShelfLife = shelfLifePeriod,
                                ExpiryDate = reader.GetDateTime(7)
                            };

                            storageProducts.Add(product);
                        }
                    }
                }
            }
        }
    }
}