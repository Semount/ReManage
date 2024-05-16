using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ReManage.Core;
using ReManage.Models;

namespace ReManage.ViewModels
{
    public class OrderViewModel : ViewModelBase
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public DateTime CreationDate { get; set; }
        public TimeSpan CreationTime { get; set; }
        public TimeSpan ServingTime { get; set; }
        public decimal Price { get; set; }
        public string Comment { get; set; }
        public ObservableCollection<OrderedDishModel> Dishes { get; set; }

        private bool _isExpanded;
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

        private ICommand _selectOrderCommand;
        public ICommand SelectOrderCommand
        {
            get => _selectOrderCommand;
            set
            {
                if (_selectOrderCommand != value)
                {
                    _selectOrderCommand = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
