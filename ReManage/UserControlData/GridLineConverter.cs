using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ReManage.UserControlData
{
    public class GridLineConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is double width) || !(values[1] is double height))
                return null;

            double gridSize = TableModel.GridSize;  // Важно, чтобы это значение было корректно определено в TableModel

            PathGeometry geometry = new PathGeometry();
            string direction = parameter as string;

            if (direction == "Horizontal")
            {
                for (double y = 0; y <= height; y += gridSize)
                {
                    var line = new LineGeometry(new Point(0, y), new Point(width, y));
                    geometry.AddGeometry(line);
                }
            }
            else if (direction == "Vertical")
            {
                for (double x = 0; x <= width; x += gridSize)
                {
                    var line = new LineGeometry(new Point(x, 0), new Point(x, height));
                    geometry.AddGeometry(line);
                }
            }

            return geometry;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}