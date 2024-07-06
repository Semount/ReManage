using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Npgsql;
using ReManage.Core;
using ReManage.Models;
using ReManage.ViewModels;

namespace ReManage.UserControlData
{
    public partial class ChefOrders : UserControl
    {
        public ObservableCollection<OrderViewModel> Orders { get; set; }
        public ICommand SelectOrderCommand { get; }
        public ICommand MarkOrderReadyCommand { get; }
        public ICommand RefreshOrdersCommand { get; }

        public ChefOrders()
        {
            InitializeComponent();
            Orders = new ObservableCollection<OrderViewModel>();
            SelectOrderCommand = new RelayCommand<int>(SelectOrder);
            MarkOrderReadyCommand = new RelayCommand<int>(MarkOrderReady);
            RefreshOrdersCommand = new RelayCommand(GetRefreshOrdersCommand());
            DataContext = this;
            LoadOrders();
        }

        private void LoadOrders()
        {
            string query = "SELECT o.id, o.creation_date, o.serving_time, o.price " +
                           "FROM orders o " +
                           "WHERE o.status_id = 1"; // Статус "Готовится"

            using (NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnection.GetConnectionString()))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        Orders.Clear();
                        while (reader.Read())
                        {
                            var orderId = reader.GetInt32(0);
                            var creationDate = reader.GetDateTime(1);
                            var servingTime = reader.GetTimeSpan(2);
                            var price = reader.GetDecimal(3);

                            Orders.Add(new OrderViewModel
                            {
                                OrderId = orderId,
                                CreationDate = creationDate,
                                ServingTime = servingTime,
                                Price = price,
                                Dishes = LoadOrderedDishes(orderId)
                            });
                        }
                    }
                }
            }
        }

        private ObservableCollection<OrderedDishModel> LoadOrderedDishes(int orderId)
        {
            string query = "SELECT od.id, d.id AS dish_id, d.name, od.amount " +
                           "FROM ordered_dishes od " +
                           "JOIN dishes d ON od.dish_id = d.id " +
                           "WHERE od.order_id = @Order_id";

            ObservableCollection<OrderedDishModel> dishes = new ObservableCollection<OrderedDishModel>();

            using (NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnection.GetConnectionString()))
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Order_id", orderId);
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var orderedDish = new OrderedDishModel
                            {
                                id = reader.GetInt32(0),
                                dish_id = reader.GetInt32(1),
                                DishName = reader.GetString(2),
                                amount = reader.GetInt32(3)
                            };

                            using (var context = new RestaurantContext())
                            {
                                orderedDish.Dish = context.Dishes.Find(orderedDish.dish_id);
                            }

                            dishes.Add(orderedDish);
                        }
                    }
                }
            }

            return dishes;
        }

        private void SelectOrder(int orderId)
        {
            var parentWindow = Window.GetWindow(this);
            if (parentWindow != null && parentWindow.DataContext is ChefViewModel viewModel)
            {
                viewModel.NavigateToOrder(orderId);
            }
            else
            {
                throw new InvalidCastException("DataContext is not of type ChefViewModel.");
            }
        }

        private void MarkOrderReady(int orderId)
        {
            using (var context = new RestaurantContext())
            {
                var order = context.Orders.Find(orderId);
                if (order != null)
                {
                    order.status_id = 2; // Статус "Готов к подаче"
                    context.SaveChanges();
                    LoadOrders(); // Обновляем список заказов
                }
            }
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null)
            {
                var order = grid.DataContext as OrderViewModel;
                if (order != null)
                {
                    order.IsExpanded = !order.IsExpanded;
                }
            }
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                var order = border.DataContext as OrderViewModel;
                if (order != null)
                {
                    order.IsExpanded = !order.IsExpanded;
                }
            }
        }

        private System.Action<object> GetRefreshOrdersCommand()
        {
            return (_) => LoadOrders();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Tag.ToString(), out var orderId))
            {
                SelectOrder(orderId);
            }
        }
    }
}