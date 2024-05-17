using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using Npgsql;
using ReManage.Core;
using ReManage.Models;
using ReManage.ViewModels;

namespace ReManage.UserControlData
{
    public partial class WaiterOrders : UserControl
    {
        private int _employeeId;
        public ObservableCollection<OrderViewModel> Orders { get; set; }

        public ICommand ToggleExpandCommand { get; }
        public ICommand IncreaseAmountCommand { get; }
        public ICommand DecreaseAmountCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public ICommand DeleteOrderCommand { get; }

        public WaiterOrders(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;
            Orders = new ObservableCollection<OrderViewModel>();
            ToggleExpandCommand = new RelayCommand<int>(ToggleExpand);
            IncreaseAmountCommand = new RelayCommand<int>(IncreaseAmount);
            DecreaseAmountCommand = new RelayCommand<int>(DecreaseAmount);
            SaveChangesCommand = new RelayCommand<int>(SaveChanges);
            DeleteOrderCommand = new RelayCommand<int>(DeleteOrder);

            DataContext = this;
            LoadOrders();
        }

        private void LoadOrders()
        {
            string query = "SELECT o.id, s.name AS status_name, o.price, o.creation_date, o.creation_time, o.serving_time, o.comment " +
                           "FROM orders o " +
                           "JOIN order_status s ON o.status_id = s.id " +
                           "WHERE o.employee_id = @Emp_id";

            using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Emp_id", _employeeId);
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        Orders.Clear();
                        while (reader.Read())
                        {
                            var orderId = reader.GetInt32(0);
                            var status = reader.GetString(1);
                            var price = reader.GetDecimal(2);
                            var creationDate = reader.GetDateTime(3);
                            var creationTime = reader.GetTimeSpan(4);
                            var servingTime = reader.GetTimeSpan(5);
                            var comment = reader.IsDBNull(6) ? null : reader.GetString(6);

                            Orders.Add(new OrderViewModel
                            {
                                OrderId = orderId,
                                Status = status,
                                Price = price,
                                CreationDate = creationDate,
                                CreationTime = creationTime,
                                ServingTime = servingTime,
                                Comment = comment,
                                Dishes = LoadOrderedDishes(orderId)
                            });
                        }
                    }
                }
            }
        }

        private ObservableCollection<OrderedDishModel> LoadOrderedDishes(int orderId)
        {
            string query = "SELECT od.id, d.name, od.amount, od.order_id, od.dish_id " +
                           "FROM ordered_dishes od " +
                           "JOIN dishes d ON od.dish_id = d.id " +
                           "WHERE od.order_id = @Order_id";

            ObservableCollection<OrderedDishModel> dishes = new ObservableCollection<OrderedDishModel>();

            using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Order_id", orderId);
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var dish = new OrderedDishModel
                            {
                                id = reader.GetInt32(0),
                                DishName = reader.GetString(1),
                                amount = reader.GetInt32(2),
                                order_id = reader.GetInt32(3),
                                dish_id = reader.GetInt32(4)
                            };

                            var order = Orders.FirstOrDefault(o => o.OrderId == orderId);
                            dish.Order = order;

                            dishes.Add(dish);
                        }
                    }
                }
            }

            return dishes;
        }

        private void ToggleExpand(int orderId)
        {
            var order = Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                order.IsExpanded = !order.IsExpanded;
            }
        }

        private void IncreaseAmount(int dishId)
        {
            var dish = Orders.SelectMany(o => o.Dishes).FirstOrDefault(d => d.id == dishId);
            if (dish != null)
            {
                dish.amount++;
            }
        }

        private void DecreaseAmount(int dishId)
        {
            var dish = Orders.SelectMany(o => o.Dishes).FirstOrDefault(d => d.id == dishId);
            if (dish != null && dish.amount > 0)
            {
                dish.amount--;
            }
        }

        private void SaveChanges(int orderId)
        {
            var order = Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                string updateOrderQuery = "UPDATE orders SET comment = @Comment WHERE id = @OrderId";
                string updateDishQuery = "UPDATE ordered_dishes SET amount = @Amount WHERE id = @DishId";

                using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (NpgsqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (NpgsqlCommand orderCommand = new NpgsqlCommand(updateOrderQuery, connection))
                            {
                                orderCommand.Parameters.AddWithValue("@Comment", (object)order.Comment ?? DBNull.Value);
                                orderCommand.Parameters.AddWithValue("@OrderId", orderId);
                                orderCommand.ExecuteNonQuery();
                            }

                            foreach (var dish in order.Dishes)
                            {
                                using (NpgsqlCommand dishCommand = new NpgsqlCommand(updateDishQuery, connection))
                                {
                                    dishCommand.Parameters.AddWithValue("@Amount", dish.amount);
                                    dishCommand.Parameters.AddWithValue("@DishId", dish.id);
                                    dishCommand.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

                LoadOrders();
            }
        }

        private void DeleteOrder(int orderId)
        {
            string deleteOrderedDishesQuery = "DELETE FROM ordered_dishes WHERE order_id = @OrderId";
            string deleteOrderQuery = "DELETE FROM orders WHERE id = @OrderId";

            using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (NpgsqlCommand orderedDishesCommand = new NpgsqlCommand(deleteOrderedDishesQuery, connection))
                        {
                            orderedDishesCommand.Parameters.AddWithValue("@OrderId", orderId);
                            orderedDishesCommand.ExecuteNonQuery();
                        }

                        using (NpgsqlCommand orderCommand = new NpgsqlCommand(deleteOrderQuery, connection))
                        {
                            orderCommand.Parameters.AddWithValue("@OrderId", orderId);
                            orderCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            LoadOrders();
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
    }
}