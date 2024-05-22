using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ReManage.Converters
{
    public class ExpiryDateToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime expiryDate)
            {
                var daysUntilExpiry = (expiryDate - DateTime.Today).TotalDays;
                if (daysUntilExpiry < 0)
                    return Brushes.Red; // Продукт просрочен
                if (daysUntilExpiry <= 2)
                    return Brushes.Yellow; // Срок годности истекает через два дня или меньше
                return Brushes.Transparent; // Нет изменений
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}