using System.Windows;
using System.Windows.Controls;
using ReManage.ViewModels;

namespace ReManage.Views
{
    /// <summary>
    /// Interaction logic for MainMenuWindow.xaml
    /// </summary>
    public partial class MainMenuWindow : Window
    {
        public MainMenuWindow()
        {
            InitializeComponent(); 
            DataContext = new LoginViewModel();
        }
       
    }
}
