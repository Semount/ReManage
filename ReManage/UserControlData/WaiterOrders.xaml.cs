using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;
using Npgsql;
using ReManage.Core;

namespace ReManage.UserControlData
{
    public partial class WaiterOrders : UserControl
    {
        private int _employeeId;

        public WaiterOrders(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;
            LoadOrders();
        }

        private void LoadOrders()
        {
            string query = "SELECT status_name AS Статус, price AS Цена, approximate_serving AS \"Предварительное время подачи\" FROM waiter_orders WHERE employee_id = @Emp_id";

            using (NpgsqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Emp_id", _employeeId);
                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                    {
                        DataTable ordersTable = new DataTable();
                        adapter.Fill(ordersTable);
                        OrdersDataGrid.ItemsSource = ordersTable.DefaultView;
                    }
                }
            }
        }
    }
}