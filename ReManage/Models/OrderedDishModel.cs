using System.ComponentModel.DataAnnotations.Schema;
using ReManage.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReManage.UserControlData;
using ReManage.ViewModels;
using System.Windows.Media.Imaging;

namespace ReManage.Models
{
    [Table("ordered_dishes")]
    public class OrderedDishModel : ViewModelBase, INotifyPropertyChanged
    {
        [Column("id")]
        public int id { get; set; }

        [Column("order_id")]
        public int order_id { get; set; }

        [Column("dish_id")]
        public int dish_id { get; set; }

        private int _amount;
        [Column("amount")]
        public int amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _dishName;
        [NotMapped]
        public string DishName
        {
            get => _dishName;
            set => SetProperty(ref _dishName, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private DishModel _dish;
        [NotMapped]
        public DishModel Dish
        {
            get => _dish;
            set
            {
                _dish = value;
                DishName = _dish?.Name;
                OnPropertyChanged(nameof(Dish));
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        [NotMapped]
        public BitmapImage ImageSource => Dish?.ImageSource;

        private OrderViewModel _order;
        [NotMapped]
        public OrderViewModel Order
        {
            get => _order;
            set
            {
                if (_order != value)
                {
                    _order = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isExpanded;
        [NotMapped]
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}