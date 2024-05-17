using ReManage.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ReManage.Views
{
    public partial class OrderCreationWindow : Window
    {
        public OrderCreationWindow(int employeeId, int tableNumber)
        {
            InitializeComponent();
            DataContext = new OrderCreationViewModel(employeeId, tableNumber, this);
        }
        private void OrderCommentTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text == "Комментарий")
            {
                textBox.Text = string.Empty;
                textBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void OrderCommentTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Комментарий";
                textBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}