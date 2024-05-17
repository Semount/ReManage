using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Text;
using ReManage.Core;
using ReManage.Models;

public class OrderCreationViewModel : ViewModelBase
{
    private int _employeeId;
    private int _tableNumber;
    private RestaurantContext _context;
    private CategoryModel _selectedCategory;
    private ObservableCollection<DishModel> _dishes;
    private ObservableCollection<CategoryModel> _categories;
    private ObservableCollection<DishModel> _selectedDishes;
    private ObservableCollection<OrderedDishModel> _orderedDishes;
    private OrderedDishModel _selectedOrderedDish;
    private string _comment;

    public ObservableCollection<DishModel> Dishes
    {
        get => _dishes;
        set => SetProperty(ref _dishes, value);
    }

    public ObservableCollection<CategoryModel> Categories
    {
        get => _categories;
        set => SetProperty(ref _categories, value);
    }

    public ObservableCollection<DishModel> SelectedDishes
    {
        get => _selectedDishes;
        set => SetProperty(ref _selectedDishes, value);
    }

    public ObservableCollection<OrderedDishModel> OrderedDishes
    {
        get => _orderedDishes;
        set => SetProperty(ref _orderedDishes, value);
    }

    public OrderedDishModel SelectedOrderedDish
    {
        get => _selectedOrderedDish;
        set => SetProperty(ref _selectedOrderedDish, value);
    }

    public CategoryModel SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value) && _selectedCategory != null)
            {
                LoadDishesByCategory(_selectedCategory.Id);
            }
        }
    }

    public string Comment
    {
        get => _comment;
        set => SetProperty(ref _comment, value);
    }

    public ICommand AddDishCommand { get; }
    public ICommand PlaceOrderCommand { get; }
    public ICommand ShowAllDishesCommand { get; }
    public ICommand IncrementOrderedDishCommand { get; }
    public ICommand DecrementOrderedDishCommand { get; }
    public ICommand RemoveOrderedDishCommand { get; }

    private Window _window;
    public Window Window
    {
        get => _window;
        set => SetProperty(ref _window, value);
    }

    public OrderCreationViewModel(int employeeId, int tableNumber, Window window)
    {
        _employeeId = employeeId;
        _tableNumber = tableNumber;
        _context = new RestaurantContext();
        _selectedDishes = new ObservableCollection<DishModel>();
        _orderedDishes = new ObservableCollection<OrderedDishModel>();
        Window = window;

        AddDishCommand = new RelayCommand(AddDish);
        PlaceOrderCommand = new RelayCommand(PlaceOrder);
        ShowAllDishesCommand = new RelayCommand(_ => LoadAllDishes());
        IncrementOrderedDishCommand = new RelayCommand(IncrementOrderedDish);
        DecrementOrderedDishCommand = new RelayCommand(DecrementOrderedDish);
        RemoveOrderedDishCommand = new RelayCommand(RemoveOrderedDish);

        LoadCategories();
        LoadAllDishes();
    }

    private void LoadCategories()
    {
        Categories = new ObservableCollection<CategoryModel>(_context.Categories.ToList());
    }

    private void LoadAllDishes()
    {
        var dishes = from d in _context.Dishes
                     join c in _context.Categories on d.CategoryId equals c.Id
                     select new DishModel
                     {
                         Id = d.Id,
                         CategoryId = d.CategoryId,
                         CategoryName = c.Name,
                         Name = d.Name,
                         Weight = d.Weight,
                         Price = d.Price,
                         Recipe = d.Recipe ?? string.Empty,
                         CookingTime = d.CookingTime,
                         Image = d.Image
                     };

        Dishes = new ObservableCollection<DishModel>(dishes.ToList());
    }

    private void LoadDishesByCategory(int categoryId)
    {
        var dishes = from d in _context.Dishes
                     join c in _context.Categories on d.CategoryId equals c.Id
                     where d.CategoryId == categoryId
                     select new DishModel
                     {
                         Id = d.Id,
                         CategoryId = d.CategoryId,
                         CategoryName = c.Name,
                         Name = d.Name,
                         Weight = d.Weight,
                         Price = d.Price,
                         Recipe = d.Recipe ?? string.Empty,
                         CookingTime = d.CookingTime,
                         Image = d.Image
                     };

        Dishes = new ObservableCollection<DishModel>(dishes.ToList());
    }

    private void AddDish(object parameter)
    {
        if (parameter is DishModel dish)
        {
            var orderedDish = OrderedDishes.FirstOrDefault(od => od.dish_id == dish.Id);
            if (orderedDish != null)
            {
                orderedDish.amount++;
            }
            else
            {
                OrderedDishes.Add(new OrderedDishModel
                {
                    dish_id = dish.Id,
                    DishName = dish.Name,
                    amount = 1
                });
            }

            // Убедимся, что блюдо также добавлено в SelectedDishes
            if (!SelectedDishes.Contains(dish))
            {
                SelectedDishes.Add(dish);
            }
        }
    }

    private void IncrementOrderedDish(object parameter)
    {
        if (parameter is OrderedDishModel orderedDish)
        {
            orderedDish.amount++;
        }
    }

    private void DecrementOrderedDish(object parameter)
    {
        if (parameter is OrderedDishModel orderedDish && orderedDish.amount > 1)
        {
            orderedDish.amount--;
        }
    }

    private void RemoveOrderedDish(object parameter)
    {
        if (parameter is OrderedDishModel orderedDish)
        {
            OrderedDishes.Remove(orderedDish);

            // Также удалим блюдо из SelectedDishes, если оно больше не заказано
            var correspondingDish = SelectedDishes.FirstOrDefault(d => d.Id == orderedDish.dish_id);
            if (correspondingDish != null)
            {
                SelectedDishes.Remove(correspondingDish);
            }
        }
    }

    private void PlaceOrder(object parameter)
    {
        if (OrderedDishes.Any())
        {
            decimal totalPrice = OrderedDishes.Sum(od => od.amount * SelectedDishes.FirstOrDefault(d => d.Id == od.dish_id)?.Price ?? 0);
            NodaTime.Duration maxCookingTime = NodaTime.Duration.FromMinutes(OrderedDishes.Max(od => {
                var dish = SelectedDishes.FirstOrDefault(d => d.Id == od.dish_id);
                if (dish?.CookingTime.HasValue ?? false)
                {
                    return (long)dish.CookingTime.Value.TotalMinutes;
                }
                return 0; // Возвращаем 0, если время готовки не определено
            }));

            OrderModel newOrder = new OrderModel
            {
                employee_id = _employeeId,
                status_id = 1,
                price = totalPrice,
                creation_date = DateTime.UtcNow.Date,
                creation_time = DateTime.UtcNow.TimeOfDay,
                serving_time = DateTime.UtcNow.TimeOfDay.Add(maxCookingTime.ToTimeSpan()),
                comment = Comment
            };

            newOrder.SetCreationDateToUtc();

            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            foreach (var dish in OrderedDishes)
            {
                OrderedDishModel orderedDish = new OrderedDishModel
                {
                    order_id = newOrder.id,
                    dish_id = dish.dish_id,
                    amount = dish.amount
                };
                _context.OrderedDishes.Add(orderedDish);
            }

            _context.SaveChanges();
            MessageBox.Show("Заказ успешно создан!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            Window.Close();
        }
        else
        {
            MessageBox.Show("Не выбрано ни одно блюдо!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

