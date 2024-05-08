using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ReManage.Core;
using ReManage.UserControlData;

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
                // Загрузите соответствующее содержимое
                CurrentContent = Activator.CreateInstance(value.ContentType);
            }
        }
    }

    private object _currentContent;
    public object CurrentContent
    {
        get { return _currentContent; }
        set { SetProperty(ref _currentContent, value); }
    }

    public ICommand CloseCommand { get; }

    public ObservableCollection<NavigationItem> SideMenuItems { get; } = new ObservableCollection<NavigationItem>();

    public ChefViewModel(string name, string surname)
    {
        CloseCommand = new RelayCommand(_ => CloseWindow());

        InitializeSideMenuItems();

        if (CurrentContent == null)
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

        SideMenuItems.Add(new NavigationItem("Все заказы", ordersIcon, typeof(UserControlChefOrders)));
        SideMenuItems.Add(new NavigationItem("Текущий заказ", currentOrderIcon, typeof(UserControlChefCurrentOrder)));
    }
}