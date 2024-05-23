using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ReManage.Core;
using ReManage.UserControlData;

namespace ReManage.ViewModels
{
    public class ChefViewModel : ViewModelBase
    {
        private NavigationItem _selectedNavigationItem;
        public NavigationItem SelectedNavigationItem
        {
            get { return _selectedNavigationItem; }
            set
            {
                if (SetProperty(ref _selectedNavigationItem, value))
                {
                    if (value.ContentType == typeof(ChefCurrentOrder))
                    {
                        // Если заказ не выбран, используем CurrentOrderId
                        CurrentContent = new ChefCurrentOrder(CurrentOrderId);
                    }
                    else
                    {
                        CurrentContent = Activator.CreateInstance(value.ContentType);
                    }
                }
            }
        }

        private object _currentContent;
        public object CurrentContent
        {
            get { return _currentContent; }
            set { SetProperty(ref _currentContent, value); }
        }

        public static int CurrentOrderId { get; set; }

        public ICommand CloseCommand { get; }

        public ObservableCollection<NavigationItem> SideMenuItems { get; } = new ObservableCollection<NavigationItem>();

        public ChefViewModel(string name, string surname)
        {
            CloseCommand = new RelayCommand(_ => CloseWindow());

            InitializeSideMenuItems();

            // Устанавливаем CurrentContent в первую вкладку бокового меню
            SelectedNavigationItem = SideMenuItems.FirstOrDefault();
            if (SelectedNavigationItem != null)
            {
                CurrentContent = Activator.CreateInstance(SelectedNavigationItem.ContentType);
            }
            else
            {
                CurrentContent = $"Здравствуйте, {name} {surname}.\nПожалуйста, воспользуйтесь навигационным меню слева для начала работы.";
            }
        }

        private void CloseWindow()
        {
            Application.Current.Windows
                .OfType<Window>()
                .SingleOrDefault(w => w.IsActive)?
                .Close();
        }

        private void InitializeSideMenuItems()
        {
            ImageSource ordersIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Order.png"));
            ImageSource currentOrderIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/CurrentOrder.png"));

            SideMenuItems.Add(new NavigationItem("Все заказы", ordersIcon, typeof(ChefOrders)));
            SideMenuItems.Add(new NavigationItem("Текущий заказ", currentOrderIcon, typeof(ChefCurrentOrder)));
        }

        public void NavigateToOrder(int orderId)
        {
            CurrentOrderId = orderId;
            CurrentContent = new ChefCurrentOrder(orderId);
        }
    }
}