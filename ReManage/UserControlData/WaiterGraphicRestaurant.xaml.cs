using ReManage.ViewModels;
using ReManage.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReManage.UserControlData
{
    public partial class WaiterGraphicRestaurant : UserControl
    {
        private int _employeeId;

        public WaiterGraphicRestaurant(int employeeId)
        {
            _employeeId = employeeId;
            InitializeComponent();
            DataContext = new RestaurantViewModel();
        }

        private void Table_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid grid && grid.DataContext is TableModel table)
            {
                var orderCreationWindow = new OrderCreationWindow(_employeeId, table.Number);
                orderCreationWindow.ShowDialog();
            }
        }
    }
}