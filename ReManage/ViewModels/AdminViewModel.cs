using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ReManage.Core;
using ReManage.UserControlData;

public class AdminViewModel : ViewModelBase
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

    public AdminViewModel(string name, string surname)
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
        ImageSource restaurantOverviewIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/RestaurantOverview.png"));
        ImageSource overviewIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Overview.png"));
        ImageSource storageIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/storage.png"));
        ImageSource employeesIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/employees.png"));
        ImageSource menuIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/menu.png"));

        SideMenuItems.Add(new NavigationItem("Обзор ресторана", restaurantOverviewIcon, typeof(AdminRestaurantOverlay)));
        SideMenuItems.Add(new NavigationItem("Сводка", overviewIcon, typeof(AdminOverview)));
        SideMenuItems.Add(new NavigationItem("Меню", menuIcon, typeof(AdminMenu)));
        SideMenuItems.Add(new NavigationItem("Склад", storageIcon, typeof(AdminStorage)));
        SideMenuItems.Add(new NavigationItem("Сотрудники", employeesIcon, typeof(AdminEmployees)));
    }
}