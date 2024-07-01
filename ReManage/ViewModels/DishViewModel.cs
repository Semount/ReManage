using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ReManage.Core;
using ReManage.Models;

namespace ReManage.ViewModels
{
    public class DishViewModel : ViewModelBase
    {
        private readonly DishModel _dish;
        private int _amount;
        private bool _isExpanded;
        private ObservableCollection<CompositionModel> _ingredients;

        public DishViewModel(DishModel dish)
        {
            _dish = dish;
            Ingredients = new ObservableCollection<CompositionModel>();
        }

        // Свойства из DishModel
        public int Id => _dish.Id;
        public string Name => _dish.Name;
        public string Recipe => _dish.Recipe;
        public int Weight => _dish.Weight;
        public decimal Price => _dish.Price;
        public TimeSpan? CookingTime => _dish.CookingTime;
        public BitmapImage ImageSource => _dish.ImageSource;

        // Специфичные для ViewModel свойства
        public int Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public ObservableCollection<CompositionModel> Ingredients
        {
            get => _ingredients;
            set => SetProperty(ref _ingredients, value);
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        private ICommand _toggleExpandCommand;
        public ICommand ToggleExpandCommand =>
            _toggleExpandCommand ??= new RelayCommand(_ => IsExpanded = !IsExpanded);
    }
}