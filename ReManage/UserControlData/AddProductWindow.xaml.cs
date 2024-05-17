using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
            LoadProducts();
        }

        private void AddProductWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RadioButton_Checked(null, null);
        }

        private void LoadProducts()
        {
            using (var context = new RestaurantContext())
            {
                try
                {
                    availableProducts = context.Products.ToList();
                }
                catch { /* Подавляем ошибки */ }
                UpdateProductComboBox();
            }
        }

        private void UpdateProductComboBox()
        {
            using (var context = new RestaurantContext())
            {
                try
                {
                    if (RefrigeratorRadioButton.IsChecked == true)
                    {
                        var refrigeratorProductIds = context.Refrigerators.Select(r => r.ProductId).ToList();
                        ProductComboBox.ItemsSource = availableProducts.Where(p => refrigeratorProductIds.Contains(p.Id)).ToList();
                        ProductType = "Refrigerator";
                    }
                    else
                    {
                        var storageProductIds = context.Storages.Select(s => s.ProductId).ToList();
                        ProductComboBox.ItemsSource = availableProducts.Where(p => storageProductIds.Contains(p.Id)).ToList();
                        ProductType = "Storage";
                    }
                    ProductComboBox.DisplayMemberPath = "Name";
                    ProductComboBox.SelectedValuePath = "Id";
                }
                catch { /* Подавляем ошибки */ }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProductComboBox();
        }

        private void ProductComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            using (var context = new RestaurantContext())
            {
                try
                {
                    if (ProductComboBox.SelectedItem is ProductModel selectedProduct)
                    {
                        if (ProductType == "Refrigerator")
                        {
                            var productDetails = context.Refrigerators.FirstOrDefault(r => r.ProductId == selectedProduct.Id);
                            if (productDetails != null)
                            {
                                ShelfLifeTextBox.Text = productDetails.ShelfLife.ToString();
                            }
                        }
                        else if (ProductType == "Storage")
                        {
                            var productDetails = context.Storages.FirstOrDefault(s => s.ProductId == selectedProduct.Id);
                            if (productDetails != null)
                            {
                                ShelfLifeTextBox.Text = productDetails.ShelfLife.ToString();
                            }
                        }
                    }
                }
                catch { /* Подавляем ошибки */ }
            }
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(AmountTextBox.Text, out int amount) || amount <= 0)
                {
                    return;
                }

                if (!DateTime.TryParse(DateDeliveredPicker.Text, out DateTime dateDelivered))
                {
                    return;
                }

                int selectedProductId = (int)ProductComboBox.SelectedValue;

                if (ProductType == "Refrigerator")
                {
                    using (var connection = DatabaseConnection.GetConnection())
                    {
                        connection.Open();
                        string query = @"
                            INSERT INTO refrigerator (product_id, amount, date_delivered, shelf_life, unfreeze_time)
                            SELECT @product_id, @amount, @date_delivered, shelf_life, unfreeze_time
                            FROM refrigerator
                            WHERE product_id = @product_id
                            LIMIT 1;";
                        using (var command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@product_id", selectedProductId);
                            command.Parameters.AddWithValue("@amount", amount);
                            command.Parameters.AddWithValue("@date_delivered", dateDelivered);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                else if (ProductType == "Storage")
                {
                    using (var connection = DatabaseConnection.GetConnection())
                    {
                        connection.Open();
                        string query = @"
                            INSERT INTO storage (product_id, amount, date_delivered, shelf_life)
                            SELECT @product_id, @amount, @date_delivered, shelf_life
                            FROM storage
                            WHERE product_id = @product_id
                            LIMIT 1;";
                        using (var command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@product_id", selectedProductId);
                            command.Parameters.AddWithValue("@amount", amount);
                            command.Parameters.AddWithValue("@date_delivered", dateDelivered);
                            command.ExecuteNonQuery();
                        }
                    }
                }

                this.Close();
            }
            catch { /* Подавляем ошибки */ }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}