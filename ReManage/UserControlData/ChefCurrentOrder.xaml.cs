using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
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

                // Получаем все заказанные блюда
                var orderedDishModels = context.OrderedDishes
                                               .Where(od => od.order_id == _orderId)
                                               .ToList();

                Dishes = new ObservableCollection<DishViewModel>();

                foreach (var orderedDish in orderedDishModels)
                {
                    var dish = context.Dishes.Find(orderedDish.dish_id);

                    if (dish == null)
                        continue;

                    // Получаем все ингредиенты и корректируем их количество в зависимости от количества заказанных блюд
                    var ingredients = context.Compositions
                                             .Where(c => c.DishId == dish.Id)
                                             .Join(context.Products,
                                                   c => c.ProductId,
                                                   p => p.Id,
                                                   (c, p) => new CompositionModel
                                                   {
                                                       Id = c.Id,
                                                       DishId = c.DishId,
                                                       ProductId = p.Id,
                                                       Amount = c.Amount * orderedDish.amount, // Масштабируем количество ингредиентов
                                                       Product = new ProductModel
                                                       {
                                                           Id = p.Id,
                                                           Name = p.Name,
                                                           Weight = p.Weight
                                                       }
                                                   })
                                             .ToList();

                    var viewModel = new DishViewModel(dish)
                    {
                        Amount = orderedDish.amount,
                        Ingredients = new ObservableCollection<CompositionModel>(ingredients),
                        IsExpanded = false
                    };
                    Dishes.Add(viewModel);
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
                    // Получаем все заказанные блюда
                    var orderedDishes = context.OrderedDishes
                                               .Where(od => od.order_id == _orderId)
                                               .ToList();

                    // Для каждого блюда в заказе
                    foreach (var orderedDish in orderedDishes)
                    {
                        var dish = context.Dishes.Find(orderedDish.dish_id);
                        if (dish != null)
                        {
                            // Получаем состав блюда (ингредиенты и их количество)
                            var compositions = context.Compositions
                                                      .Where(c => c.DishId == dish.Id)
                                                      .ToList();

                            // Для каждого ингредиента блюда
                            foreach (var composition in compositions)
                            {
                                var productId = composition.ProductId;
                                var requiredAmount = composition.Amount * orderedDish.amount;

                                // Ищем продукт в холодильнике
                                var refrigeratorProduct = context.Refrigerators
                                    .Where(rp => rp.ProductId == productId)
                                    .Select(rp => new
                                    {
                                        rp.Id,
                                        rp.ProductId,
                                        rp.Amount,
                                        rp.DateDelivered,
                                        rp.UnfreezeTime,
                                        rp.ExpiryDate
                                    })
                                    .ToList()
                                    .FirstOrDefault();

                                // Если продукт не найден в холодильнике, ищем его на складе
                                if (refrigeratorProduct == null)
                                {
                                    var storageProduct = context.Storages
                                        .Where(sp => sp.ProductId == productId)
                                        .Select(sp => new
                                        {
                                            sp.Id,
                                            sp.ProductId,
                                            Amount = sp.Amount,
                                            sp.DateDelivered,
                                            sp.ExpiryDate
                                        })
                                        .ToList()
                                        .FirstOrDefault();

                                    if (storageProduct != null)
                                    {
                                        context.Database.ExecuteSqlRaw("UPDATE storage SET amount = amount - {0} WHERE id = {1}", requiredAmount, storageProduct.Id);
                                    }
                                }
                                else
                                {
                                    context.Database.ExecuteSqlRaw("UPDATE refrigerator SET amount = amount - {0} WHERE id = {1}", requiredAmount, refrigeratorProduct.Id);
                                }
                            }
                        }
                    }

                    order.status_id = 2; // Статус "Готов к подаче"
                    context.SaveChanges();
                    LoadOrderData();
                    // Переключение обратно на страницу с выбором заказов
                    var parentWindow = Window.GetWindow(this);
                    if (parentWindow != null && parentWindow.DataContext is ChefViewModel viewModel)
                    {
                        viewModel.NavigateToOrders();
                    }
                    else
                    {
                        throw new InvalidCastException("DataContext is not of type ChefViewModel.");
                    }
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