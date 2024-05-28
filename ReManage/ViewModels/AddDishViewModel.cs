using ReManage.Models;
using ReManage.Core;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Collections.Generic;

namespace ReManage.ViewModels
{
    public class AddDishViewModel : ViewModelBase
    {
        private string name;
        private int weight;
        private decimal price;
        private int selectedCategoryId;
        private string recipe;
        private TimeSpan cookingTime;
        private byte[] image;
        private BitmapImage imageSource;

        public AddDishViewModel(List<CategoryModel> categories, List<ProductModel> products)
        {
            Categories = categories;
            Products = products;
            DishIngredients = new ObservableCollection<CompositionModel>();
            LoadImageCommand = new RelayCommand(LoadImage);
            AddDishCommand = new RelayCommand(AddDish);
            AddIngredientCommand = new RelayCommand(AddIngredient);
            RemoveIngredientCommand = new RelayCommand(RemoveIngredient);
        }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public int Weight
        {
            get => weight;
            set => SetProperty(ref weight, value);
        }

        public decimal Price
        {
            get => price;
            set => SetProperty(ref price, value);
        }

        public int SelectedCategoryId
        {
            get => selectedCategoryId;
            set => SetProperty(ref selectedCategoryId, value);
        }

        public string Recipe
        {
            get => recipe;
            set => SetProperty(ref recipe, value);
        }

        public TimeSpan CookingTime
        {
            get => cookingTime;
            set => SetProperty(ref cookingTime, value);
        }

        public byte[] Image
        {
            get => image;
            set
            {
                if (SetProperty(ref image, value))
                {
                    ImageSource = LoadImageFromBytes(value);
                }
            }
        }

        public BitmapImage ImageSource
        {
            get => imageSource;
            set => SetProperty(ref imageSource, value);
        }

        public List<CategoryModel> Categories { get; }
        public List<ProductModel> Products { get; }
        public ObservableCollection<CompositionModel> DishIngredients { get; }

        public ICommand LoadImageCommand { get; }
        public ICommand AddDishCommand { get; }
        public ICommand AddIngredientCommand { get; }
        public ICommand RemoveIngredientCommand { get; }

        private void LoadImage(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                Image = File.ReadAllBytes(openFileDialog.FileName);
            }
        }

        private void AddDish(object parameter)
        {
            if (ValidateInputs(out string errorMessage))
            {
                var newDish = new DishModel
                {
                    Name = Name,
                    Weight = Weight,
                    Price = Price,
                    CategoryId = SelectedCategoryId,
                    Recipe = Recipe,
                    CookingTime = CookingTime,
                    Image = Image
                };

                using (var context = new RestaurantContext())
                {
                    context.Dishes.Add(newDish);
                    context.SaveChanges();

                    foreach (var ingredient in DishIngredients)
                    {
                        ingredient.DishId = newDish.Id;
                        context.Compositions.Add(ingredient);
                    }

                    context.SaveChanges();
                }

                MessageBox.Show("Блюдо добавлено успешно.");
                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
            }
            else
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddIngredient(object parameter)
        {
            DishIngredients.Add(new CompositionModel());
        }

        private void RemoveIngredient(object parameter)
        {
            if (parameter is CompositionModel ingredient)
            {
                DishIngredients.Remove(ingredient);
            }
        }

        private bool ValidateInputs(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Name))
            {
                errorMessage = "Название не может быть пустым.";
                return false;
            }
            if (!int.TryParse(Weight.ToString(), out _))
            {
                errorMessage = "Вес должен быть числом.";
                return false;
            }
            if (!decimal.TryParse(Price.ToString(), out _))
            {
                errorMessage = "Цена должна быть числом.";
                return false;
            }
            if (SelectedCategoryId == 0)
            {
                errorMessage = "Выберите категорию.";
                return false;
            }
            if (!TimeSpan.TryParse(CookingTime.ToString(), out _))
            {
                errorMessage = "Время приготовления должно быть в формате hh:mm:ss.";
                return false;
            }

            return true;
        }

        private BitmapImage LoadImageFromBytes(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
            {
                return null;
            }

            using (var ms = new MemoryStream(imageData))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }
    }
}