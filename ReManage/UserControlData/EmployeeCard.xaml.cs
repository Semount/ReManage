using System.Windows;
using System.Windows.Controls;
using ReManage.Models;
using ReManage.Core;

namespace ReManage.UserControlData
{
    public partial class EmployeeCard : UserControl
    {
        public EmployeeCard()
        {
            InitializeComponent();
        }

        private void DeleteEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите удалить этого сотрудника?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes && DataContext is EmployeeModel employee)
            {
                OnEmployeeDeleted(employee);
            }
        }

        public delegate void EmployeeDeletedEventHandler(EmployeeModel employee);
        public event EmployeeDeletedEventHandler EmployeeDeleted;

        protected void OnEmployeeDeleted(EmployeeModel employee)
        {
            EmployeeDeleted?.Invoke(employee);
        }
    }
}