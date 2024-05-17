using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ReManage.Models;
using ReManage.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace ReManage.UserControlData
{
    public partial class AdminEmployees : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<EmployeeModel> Employees { get; set; }
        public ObservableCollection<RoleModel> Roles { get; set; }

        private bool _isAddEmployeePopupOpen;
        public bool IsAddEmployeePopupOpen
        {
            get { return _isAddEmployeePopupOpen; }
            set
            {
                _isAddEmployeePopupOpen = value;
                OnPropertyChanged();
            }
        }

        public AdminEmployees()
        {
            InitializeComponent();
            DataContext = this;
            LoadEmployees();
            LoadRoles();
        }

        private void LoadEmployees()
        {
            using (var context = new RestaurantContext())
            {
                Employees = new ObservableCollection<EmployeeModel>(context.Employees.Include("Role").ToList());
            }

            foreach (var employee in Employees)
            {
                var card = new EmployeeCard
                {
                    DataContext = employee
                };
                card.EmployeeDeleted += OnEmployeeDeleted;
                EmployeeCardsPanel.Children.Add(card);
            }
        }

        private void LoadRoles()
        {
            using (var context = new RestaurantContext())
            {
                Roles = new ObservableCollection<RoleModel>(context.Roles.ToList());
            }
            RoleComboBox.ItemsSource = Roles;
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            IsAddEmployeePopupOpen = true;
        }

        private void CancelAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            IsAddEmployeePopupOpen = false;
        }

        private void ConfirmAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            var newEmployee = new EmployeeModel
            {
                RoleId = (int)RoleComboBox.SelectedValue,
                Name = NameTextBox.Text,
                Surname = SurnameTextBox.Text,
                Login = LoginTextBox.Text,
                Password = PasswordBox.Password
            };

            using (var context = new RestaurantContext())
            {
                context.Employees.Add(newEmployee);
                context.SaveChanges();
            }

            Employees.Add(newEmployee);
            var card = new EmployeeCard
            {
                DataContext = newEmployee
            };
            card.EmployeeDeleted += OnEmployeeDeleted;
            EmployeeCardsPanel.Children.Add(card);
            IsAddEmployeePopupOpen = false;
        }

        private void OnEmployeeDeleted(EmployeeModel employee)
        {
            using (var context = new RestaurantContext())
            {
                var employeeToDelete = context.Employees.FirstOrDefault(e => e.Id == employee.Id);
                if (employeeToDelete != null)
                {
                    context.Employees.Remove(employeeToDelete);
                    context.SaveChanges();
                }
            }

            var cardToRemove = EmployeeCardsPanel.Children.OfType<EmployeeCard>().FirstOrDefault(c => c.DataContext == employee);
            if (cardToRemove != null)
            {
                EmployeeCardsPanel.Children.Remove(cardToRemove);
                Employees.Remove(employee);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == tb.Tag.ToString())
            {
                tb.Text = string.Empty;
                tb.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = tb.Tag.ToString();
                tb.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox pb = (PasswordBox)sender;
            if (pb.Password == pb.Tag.ToString())
            {
                pb.Password = string.Empty;
                pb.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox pb = (PasswordBox)sender;
            if (string.IsNullOrWhiteSpace(pb.Password))
            {
                pb.Password = pb.Tag.ToString();
                pb.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
            }
        }
    }
}