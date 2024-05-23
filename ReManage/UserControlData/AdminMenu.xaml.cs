using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ReManage.Models;
using ReManage.Core;

namespace ReManage.UserControlData
{
    public partial class AdminMenu : UserControl
    {
        private List<DishModel> dishes;
        private List<CategoryModel> categories;

        public AdminMenu()
        {
            InitializeComponent();
            LoadCategories();
            LoadDishes();
            CategoryComboBox.SelectedIndex = 0; // Устанавливаем "Все категории" по умолчанию
        }

        public List<CategoryModel> Categories
        {
            get { return categories; }
            set { categories = value; }
        }

        private void LoadCategories()
        {
            using (var context = new RestaurantContext())
            {
                try
                {
                    categories = context.Categories.ToList();
                    categories.Insert(0, new CategoryModel { Id = 0, Name = "Все категории" }); // Добавляем элемент для просмотра всех категорий
                    CategoryComboBox.ItemsSource = categories;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке категорий: {ex.Message}");
                }
            }
        }

        private void LoadDishes()
        {
            using (var context = new RestaurantContext())
            {
                try
                {
                    dishes = context.Dishes.ToList();
                    foreach (var dish in dishes)
                    {
                        var category = categories.FirstOrDefault(c => c.Id == dish.CategoryId);
                        dish.CategoryName = category?.Name;
                    }
                    DishesItemsControl.ItemsSource = dishes;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке блюд: {ex.Message}");
                }
            }
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCategory = CategoryComboBox.SelectedItem as CategoryModel;
            if (selectedCategory != null && selectedCategory.Id != 0)
            {
                var filteredDishes = dishes.Where(d => d.CategoryId == selectedCategory.Id).ToList();
                DishesItemsControl.ItemsSource = filteredDishes;
            }
            else
            {
                DishesItemsControl.ItemsSource = dishes;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var dish = button?.Tag as DishModel;
            if (dish != null)
            {
                using (var context = new RestaurantContext())
                {
                    try
                    {
                        var existingDish = context.Dishes.FirstOrDefault(d => d.Id == dish.Id);
                        if (existingDish != null)
                        {
                            existingDish.Name = dish.Name;
                            existingDish.Weight = dish.Weight;
                            existingDish.Price = dish.Price;
                            existingDish.Recipe = dish.Recipe;
                            existingDish.CookingTime = dish.CookingTime;
                            existingDish.CategoryId = dish.CategoryId;
                            context.SaveChanges();
                            MessageBox.Show("Изменения успешно сохранены.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}");
                    }
                }
            }
        }
    }
}