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
        ImageSource restaurantOverviewIcon = new BitmapImage(new Uri("pack://application:,,,/PathToYourIcon/placeholder.png"));
        ImageSource overviewIcon = new BitmapImage(new Uri("pack://application:,,,/PathToYourIcon/placeholder.png"));

        SideMenuItems.Add(new NavigationItem("Обзор ресторана", restaurantOverviewIcon, typeof(UserControlAdminRestaurantOverlay)));
        SideMenuItems.Add(new NavigationItem("Сводка", overviewIcon, typeof(UserControlAdminOverview)));
    }
}