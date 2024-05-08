using System;
using System.Windows;
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
            logoutButton.Click += LogoutButton_Click;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            HandleLogout();
        }

        private void HandleLogout()
        {
            // Получение ссылки на текущее активное окно
            var currentWindow = Window.GetWindow(this);
            // Создание и отображение главного окна
            var mainWindow = new MainWindow();
            mainWindow.Show();
            // Закрытие текущего окна
            currentWindow?.Close();
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