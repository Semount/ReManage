using ReManage.ViewModels;
using System.Windows;

namespace ReManage.Views
{
    public partial class OrderCreationWindow : Window
    {
        public OrderCreationWindow(int employeeId, int tableNumber)
        {
            InitializeComponent();
            DataContext = new OrderCreationViewModel(employeeId, tableNumber);
        }
    }
}