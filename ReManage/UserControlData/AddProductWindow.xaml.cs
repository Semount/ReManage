using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ReManage.Core;
using ReManage.Models;

namespace ReManage.UserControlData
{
    public partial class AddProductWindow : Window
    {
        private List<ProductModel> availableProducts = new List<ProductModel>();
        public string ProductType { get; private set; }

        public AddProductWindow()
        {
            InitializeComponent();
            this.Loaded += AddProductWindow_Loaded;
        }

        private async void AddProductWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadProductsAsync();
            RadioButton_Checked(null, null);
        }

        private async Task LoadProductsAsync()
        {
            using (var context = new RestaurantContext())
            {
                try
                {
                    availableProducts = await context.Products.ToListAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось загрузить список продуктов: {ex.Message}");
                    availableProducts = new List<ProductModel>(); // Установка пустого списка на случай ошибки
                }
                UpdateProductComboBox();
            }
        }

        private void UpdateProductComboBox()
        {
            try
            {
                if (ProductComboBox == null)
                {
                    // Подавляем ошибку и продолжаем выполнение
                    return;
                }

                using (var context = new RestaurantContext())
                {
                    List<int> productIds = new List<int>();
                    if (RefrigeratorRadioButton.IsChecked == true)
                    {
                        productIds = context.Refrigerators.Select(r => r.ProductId).ToList();
                        ProductType = "Refrigerator";
                    }
                    else if (StorageRadioButton.IsChecked == true)
                    {
                        productIds = context.Storages.Select(s => s.ProductId).ToList();
                        ProductType = "Storage";
                    }

                    if (productIds.Count == 0)
                    {
                        MessageBox.Show("В выбранной категории нет продуктов.");
                    }

                    if (availableProducts == null)
                    {
                        MessageBox.Show("Список доступных продуктов не загружен!");
                        return;
                    }

                    ProductComboBox.ItemsSource = availableProducts.Where(p => productIds.Contains(p.Id)).ToList();
                    ProductComboBox.DisplayMemberPath = "Name";
                    ProductComboBox.SelectedValuePath = "Id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении списка продуктов: {ex.Message}");
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProductComboBox();
        }

        private void ProductComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ProductComboBox.SelectedItem is ProductModel selectedProduct)
            {
                try
                {
                    using (var context = new RestaurantContext())
                    {
                        if (ProductType == "Refrigerator")
                        {
                            var productDetails = context.Refrigerators.FirstOrDefault(r => r.ProductId == selectedProduct.Id);
                            // ShelfLifeTextBox.Text = productDetails?.ShelfLife.ToString() ?? "Н/Д";
                        }
                        else if (ProductType == "Storage")
                        {
                            var productDetails = context.Storages.FirstOrDefault(s => s.ProductId == selectedProduct.Id);
                            // ShelfLifeTextBox.Text = productDetails?.ShelfLife.ToString() ?? "Н/Д";
                        }
                    }
                }
                catch (Exception)
                {
                    // Подавляем ошибку и продолжаем выполнение
                }
            }
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(AmountTextBox.Text, out int amount) || amount <= 0)
                {
                    MessageBox.Show("Количество должно быть положительным числом.");
                    return;
                }

                if (!DateTime.TryParse(DateDeliveredPicker.Text, out DateTime dateDelivered))
                {
                    MessageBox.Show("Введите корректную дату доставки.");
                    return;
                }

                int selectedProductId = (int)ProductComboBox.SelectedValue;
                AddProductToDatabase(selectedProductId, amount, dateDelivered);
                MessageBox.Show("Продукт успешно добавлен.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении продукта: {ex.Message}");
            }
        }

        private void AddProductToDatabase(int productId, int amount, DateTime dateDelivered)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                string query = ProductType == "Refrigerator" ?
                    "INSERT INTO refrigerator (product_id, amount, date_delivered, shelf_life, unfreeze_time) " +
                    "SELECT @product_id, @amount, @date_delivered, shelf_life, unfreeze_time FROM refrigerator WHERE product_id = @product_id LIMIT 1;" :
                    "INSERT INTO storage (product_id, amount, date_delivered, shelf_life) " +
                    "SELECT @product_id, @amount, @date_delivered, shelf_life FROM storage WHERE product_id = @product_id LIMIT 1;";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@product_id", productId);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.Parameters.AddWithValue("@date_delivered", dateDelivered);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}