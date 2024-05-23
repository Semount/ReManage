using ReManage.Core;
using ReManage.UserControlData;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

public class WaiterViewModel : ViewModelBase
{
    private NavigationItem _selectedNavigationItem;
    private int _employeeId;

    public NavigationItem SelectedNavigationItem
    {
        get { return _selectedNavigationItem; }
        set
        {
            if (SetProperty(ref _selectedNavigationItem, value))
            {
                // Загрузите соответствующее содержимое
                CurrentContent = Activator.CreateInstance(value.ContentType, _employeeId);
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

    public WaiterViewModel(int employeeId, string name, string surname)
    {
        _employeeId = employeeId;
        CloseCommand = new RelayCommand(_ => CloseWindow());

        InitializeSideMenuItems();

        // Установите CurrentContent в первую вкладку бокового меню
        SelectedNavigationItem = SideMenuItems.FirstOrDefault();
        if (SelectedNavigationItem != null)
        {
            // Создайте экземпляр соответствующего типа для CurrentContent, передавая employeeId
            CurrentContent = Activator.CreateInstance(SelectedNavigationItem.ContentType, employeeId);
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
        // Загрузка иконок
        ImageSource tableViewIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Tables.png"));
        ImageSource orderIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Order.png"));

        SideMenuItems.Add(new NavigationItem("Графическое представление", tableViewIcon, typeof(WaiterGraphicRestaurant)));
        SideMenuItems.Add(new NavigationItem("Список заказов", orderIcon, typeof(WaiterOrders)));
    }
}