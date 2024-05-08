using System;
using System.Windows.Controls;
using System.Windows.Media;
using ReManage.Core; // Используйте NavigationItem из Core

namespace ReManage.UserControlData
{
    public partial class SideMenuUserControl : UserControl
    {
        public SideMenuUserControl()
        {
            InitializeComponent();
            sideMenuListView.SelectionChanged += sideMenuListView_SelectionChanged;
        }

        public void AddMenuItem(NavigationItem navItem)
        {
            sideMenuListView.Items.Add(navItem);
        }

        public class SelectedMenuItemEventArgs : EventArgs
        {
            public string Title { get; }
            public ImageSource Icon { get; }
            public Type ContentType { get; }

            public SelectedMenuItemEventArgs(string title, ImageSource icon, Type contentType)
            {
                Title = title;
                Icon = icon;
                ContentType = contentType;
            }
        }

        public event EventHandler<SelectedMenuItemEventArgs> SelectedMenuItemChanged;

        private void OnSelectedMenuItemChanged()
        {
            if (sideMenuListView.SelectedItem is NavigationItem selectedItem)
            {
                SelectedMenuItemChanged?.Invoke(this, new SelectedMenuItemEventArgs(
                    selectedItem.Title,
                    selectedItem.Icon,
                    selectedItem.ContentType
                ));
            }
        }

        private void sideMenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSelectedMenuItemChanged();
        }
    }
}