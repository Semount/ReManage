using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using ReManage.Core;
using ReManage.Models;
using ReManage.ViewModels;

namespace ReManage.UserControlData
{
    public partial class ChefCurrentOrder : UserControl
    {
        private int _orderId;
        private OrderModel _order;
        public ObservableCollection<DishViewModel> Dishes { get; private set; }

        public ICommand MarkOrderReadyCommand { get; }

        public DateTime OrderTime
        {
            get => _order?.creation_date ?? DateTime.MinValue;
            set
            {
                if (_order != null)
                {
                    _order.creation_date = value;
                    RaisePropertyChanged(nameof(OrderTime));
                }
            }
        }

        public TimeSpan ServingTime
        {
            get => _order?.serving_time ?? TimeSpan.Zero;
            set
            {
                if (_order != null)
                {
                    _order.serving_time = value;
                    RaisePropertyChanged(nameof(ServingTime));
                }
            }
        }

        public string Comment
        {
            get => _order?.comment ?? string.Empty;
            set
            {
                if (_order != null)
                {
                    _order.comment = value;
                    RaisePropertyChanged(nameof(Comment));
                }
            }
        }

        public ChefCurrentOrder(int orderId)
        {
            InitializeComponent();
            _orderId = orderId;
            LoadOrderData();

            MarkOrderReadyCommand = new RelayCommand(param => MarkOrderReady());
            DataContext = this;
        }

        private void LoadOrderData()
        {
            using (var context = new RestaurantContext())
            {
                _order = context.Orders.Find(_orderId);
                if (_order == null)
                    return;

                var orderedDishModels = context.OrderedDishes
                                               .Where(od => od.order_id == _orderId)
                                               .ToList();

                Dishes = new ObservableCollection<DishViewModel>();

                foreach (var orderedDish in orderedDishModels)
                {
                    var dish = context.Dishes.Find(orderedDish.dish_id);

                    if (dish == null)
                        continue;

                    var ingredients = context.Compositions
                                             .Where(c => c.DishId == dish.Id)
                                             .Join(context.Products,
                                                   c => c.ProductId,
                                                   p => p.Id,
                                                   (c, p) => new ProductModel
                                                   {
                                                       Id = p.Id,
                                                       Name = p.Name,
                                                       Weight = p.Weight,
                                                       Price = p.Price
                                                   })
                                             .ToList();

                    Dishes.Add(new DishViewModel
                    {
                        Id = dish.Id,
                        Name = dish.Name,
                        Recipe = dish.Recipe,
                        Ingredients = new ObservableCollection<ProductModel>(ingredients),
                        IsExpanded = false
                    });
                }
            }

            // Обновляем привязанные свойства
            RaisePropertyChanged(nameof(OrderTime));
            RaisePropertyChanged(nameof(ServingTime));
            RaisePropertyChanged(nameof(Comment));
            RaisePropertyChanged(nameof(Dishes));
        }

        private void MarkOrderReady()
        {
            using (var context = new RestaurantContext())
            {
                var order = context.Orders.Find(_orderId);
                if (order != null)
                {
                    order.status_id = 2; // Статус "Готов к подаче"
                    context.SaveChanges();
                    LoadOrderData();
                }
            }
        }

        private void DishCard_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                var dish = border.DataContext as DishViewModel;
                if (dish != null)
                {
                    dish.IsExpanded = !dish.IsExpanded;
                }
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}