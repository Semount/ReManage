using System.Collections.ObjectModel;
using System.Windows.Input;
using ReManage.Core;
using ReManage.Models;

namespace ReManage.ViewModels
{
    public class DishViewModel : ViewModelBase
    {
        private bool _isExpanded;

        public int Id { get; set; }
        public string Name { get; set; }
        public string Recipe { get; set; }
        public ObservableCollection<ProductModel> Ingredients { get; set; }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }

        private ICommand _toggleExpandCommand;
        public ICommand ToggleExpandCommand
        {
            get => _toggleExpandCommand ??= new RelayCommand(param => IsExpanded = !IsExpanded);
        }
    }
}