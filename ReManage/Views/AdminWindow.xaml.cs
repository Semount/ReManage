using ReManage.ViewModels;
using System.Windows;
using ReManage.UserControlData;
using ReManage.Core;

namespace ReManage.Views
{
    public partial class AdminWindow : Window
    {
        public AdminWindow(string name, string surname)
        {
            InitializeComponent();
            DataContext = new AdminViewModel(name, surname);
        }

        private void SideMenuUserControl_SelectedMenuItemChanged(object sender, SideMenuUserControl.SelectedMenuItemEventArgs e)
        {
            var viewModel = DataContext as AdminViewModel;
            if (viewModel != null)
                viewModel.SelectedNavigationItem = new NavigationItem(e.Title, e.Icon, e.ContentType);
        }
    }
}