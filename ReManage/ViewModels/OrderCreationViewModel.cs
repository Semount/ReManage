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

    public OrderCreationViewModel(int employeeId, int tableNumber)
    {
        _employeeId = employeeId;
        _tableNumber = tableNumber;
        _context = new RestaurantContext();
        _selectedDishes = new ObservableCollection<DishModel>();

        AddDishCommand = new RelayCommand(AddDish);
        PlaceOrderCommand = new RelayCommand(PlaceOrder);
        ShowAllDishesCommand = new RelayCommand(_ => LoadAllDishes());

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
            SelectedDishes.Add(dish);
        }
    }

    private void PlaceOrder(object parameter)
    {
        decimal totalPrice = SelectedDishes.Sum(d => d.Price);
        NodaTime.Duration maxCookingTime = NodaTime.Duration.FromMinutes(SelectedDishes.Max(d => d.CookingTime.HasValue ? d.CookingTime.Value.TotalMinutes : 0));

        OrderModel newOrder = new OrderModel
        {
            employee_id = _employeeId,
            status_id = 1, // Идентификатор статуса "Готовится"
            price = totalPrice,
            creation_date = DateTime.UtcNow.Date,
            creation_time = DateTime.UtcNow.TimeOfDay,
            serving_time = DateTime.UtcNow.TimeOfDay.Add(maxCookingTime.ToTimeSpan()),
            comment = Comment
        };

        newOrder.SetCreationDateToUtc(); // Преобразуем дату в формат UTC

        _context.Orders.Add(newOrder);
        _context.SaveChanges();

        foreach (var dish in SelectedDishes.GroupBy(d => d.Id).Select(g => new { DishId = g.Key, Amount = g.Count() }))
        {
            OrderedDishModel orderedDish = new OrderedDishModel
            {
                // id поле не устанавливается, оно будет сгенерировано автоматически
                order_id = newOrder.id,
                dish_id = dish.DishId,
                amount = dish.Amount
            };
            _context.OrderedDishes.Add(orderedDish);
        }

        _context.SaveChanges();
        MessageBox.Show("Заказ успешно создан!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
    }

}
