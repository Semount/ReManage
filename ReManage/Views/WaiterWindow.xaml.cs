using ReManage.ViewModels;
using System.Windows;
using ReManage.UserControlData;
using ReManage.Core;

namespace ReManage.Views
{
    public partial class WaiterWindow : Window
    {
        private WaiterViewModel _waiterViewModel;

        // Изменение конструктора для принятия name и surname
        public WaiterWindow(string name, string surname)
        {
            InitializeComponent();

            // Передаем name и surname в ViewModel
            _waiterViewModel = new WaiterViewModel(name, surname);
            DataContext = _waiterViewModel;
        }

        private void SideMenuUserControl_SelectedMenuItemChanged(object sender, SideMenuUserControl.SelectedMenuItemEventArgs e)
        {
            // Обработайте выбор вкладки
            _waiterViewModel.SelectedNavigationItem = new NavigationItem(e.Title, e.Icon, e.ContentType);
        }
    }
}