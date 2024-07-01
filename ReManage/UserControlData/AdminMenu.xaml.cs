using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ReManage.Models;
using ReManage.Core;
using ReManage.ViewModels;
using ReManage.Views;

namespace ReManage.UserControlData
{
    public partial class AdminMenu : UserControl
    {
        private List<DishModel> dishes;
        private List<CategoryModel> categories;
        private DispatcherTimer searchTimer;

        public AdminMenu()
        {
            InitializeComponent();
            LoadCategories();
            LoadDishes();
            CategoryComboBox.SelectedIndex = 0; // Устанавливаем "Все категории" по умолчанию

            // Инициализация таймера для дебаунсинга поиска
            searchTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            searchTimer.Tick += SearchTimer_Tick;
        }

        public List<CategoryModel> Categories
        {
            get { return categories; }
            set { categories = value; }
        }

        public List<CategoryModel> CategoriesWithoutAll
        {
            get { return categories?.Where(c => c.Id != 0).ToList(); }
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
                    UpdateDishItemsControl();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке блюд: {ex.Message}");
                }
            }
        }

        private List<ProductModel> LoadProducts()
        {
            using (var context = new RestaurantContext())
            {
                try
                {
                    return context.Products.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке продуктов: {ex.Message}");
                    return new List<ProductModel>();
                }
            }
        }

        private void UpdateDishItemsControl()
        {
            var searchQuery = SearchTextBox.Text.ToLower();
            var selectedCategory = CategoryComboBox.SelectedItem as CategoryModel;

            // Фильтрация по названию и категории
            var filteredDishes = dishes
                .Where(d => d.Name.ToLower().Contains(searchQuery) && (selectedCategory == null || selectedCategory.Id == 0 || d.CategoryId == selectedCategory.Id))
                .OrderBy(d => d.CategoryId)
                .ToList();

            DishesItemsControl.ItemsSource = filteredDishes;
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDishItemsControl();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Перезапуск таймера при каждом изменении текста
            searchTimer.Stop();
            searchTimer.Start();
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            // Остановка таймера и обновление данных
            searchTimer.Stop();
            UpdateDishItemsControl();
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
                            existingDish.Image = dish.Image;
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var dish = button?.Tag as DishModel;
            if (dish != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить блюдо \"{dish.Name}\"?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new RestaurantContext())
                    {
                        try
                        {
                            var existingDish = context.Dishes.FirstOrDefault(d => d.Id == dish.Id);
                            if (existingDish != null)
                            {
                                // Delete related composition records
                                var compositionRecords = context.Compositions.Where(c => c.DishId == dish.Id);
                                context.Compositions.RemoveRange(compositionRecords);

                                // Delete the dish
                                context.Dishes.Remove(existingDish);

                                context.SaveChanges();
                                MessageBox.Show("Блюдо успешно удалено.");
                                LoadDishes(); // Перезагрузить блюда после удаления
                            }
                        }
                        catch (Exception ex)
                        {
                            while (ex.InnerException != null)
                            {
                                ex = ex.InnerException;
                            }
                            MessageBox.Show($"Ошибка при удалении блюда: {ex.Message}");
                        }
                    }
                }
            }
        }


        private void AddDishButton_Click(object sender, RoutedEventArgs e)
        {
            var products = LoadProducts();
            var addDishViewModel = new AddDishViewModel(categories, products);
            var addDishWindow = new AddDishWindow { DataContext = addDishViewModel };

            if (addDishWindow.ShowDialog() == true)
            {
                LoadDishes(); // Перезагрузить блюда после добавления нового
            }
        }
    }
}