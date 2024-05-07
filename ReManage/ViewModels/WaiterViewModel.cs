using ReManage.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReManage.ViewModels
{
    internal class WaiterViewModel : ViewModelBase
    {
        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                _selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
            }
        }

        private ICommand _tabSelectionChangedCommand;
        public ICommand TabSelectionChangedCommand
        {
            get
            {
                if (_tabSelectionChangedCommand == null)
                {
                    _tabSelectionChangedCommand = new RelayCommand<int>(OnTabSelectionChanged);
                }
                return _tabSelectionChangedCommand;
            }
        }

        private void OnTabSelectionChanged(int tabIndex)
        {
            SelectedTabIndex = tabIndex;
            // Здесь можно выполнить дополнительные действия при переключении вкладок
        }
    }
}
