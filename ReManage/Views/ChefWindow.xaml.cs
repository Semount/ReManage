using ReManage.ViewModels;
using System.Windows;
using ReManage.UserControlData;
using ReManage.Core;

namespace ReManage.Views
{
    public partial class ChefWindow : Window
    {
        public ChefWindow(string name, string surname)
        {
            InitializeComponent();
            DataContext = new ChefViewModel(name, surname);
        }

        private void SideMenuUserControl_SelectedMenuItemChanged(object sender, SideMenuUserControl.SelectedMenuItemEventArgs e)
        {
            var viewModel = DataContext as ChefViewModel;
            if (viewModel != null)
                viewModel.SelectedNavigationItem = new NavigationItem(e.Title, e.Icon, e.ContentType);
        }
    }
}