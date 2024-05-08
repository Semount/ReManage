using System;
using System.Windows.Media;

namespace ReManage.Core
{
    public class NavigationItem : ViewModelBase
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private ImageSource _icon;
        public ImageSource Icon
        {
            get { return _icon; }
            set { SetProperty(ref _icon, value); }
        }

        private Type _contentType;
        public Type ContentType
        {
            get { return _contentType; }
            set { SetProperty(ref _contentType, value); }
        }

        public NavigationItem(string title, ImageSource icon, Type contentType)
        {
            _title = title;
            _icon = icon;
            _contentType = contentType;
        }
    }
}