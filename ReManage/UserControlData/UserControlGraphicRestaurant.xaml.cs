using System.Windows.Controls;

namespace ReManage.UserControlData
{
    public partial class UserControlGraphicRestaurant : UserControl
    {
        public UserControlGraphicRestaurant()
        {
            InitializeComponent();
            DataContext = new RestaurantViewModel();
        }
    }
}