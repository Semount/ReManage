using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReManage.UserControlData
{
    public partial class AdminRestaurantOverlay : UserControl
    {
        private bool isDragging;
        private Point mouseOffset;
        private RestaurantViewModel viewModel;

        public AdminRestaurantOverlay()
        {
            InitializeComponent();
            viewModel = new RestaurantViewModel();
            DataContext = viewModel;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                isDragging = true;
                var mousePosition = e.GetPosition(border);
                mouseOffset = new Point(mousePosition.X, mousePosition.Y);
                border.CaptureMouse();
            }
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && sender is Border border && border.DataContext is TableModel table)
            {
                var mousePosition = e.GetPosition(canvas);

                // Вычисляем новые координаты с учетом смещения мыши
                double newX = mousePosition.X - mouseOffset.X;
                double newY = mousePosition.Y - mouseOffset.Y;

                // Ограничиваем координаты, чтобы не выходить за пределы Canvas
                newX = Clamp(newX, 0, canvas.ActualWidth - border.Width);
                newY = Clamp(newY, 0, canvas.ActualHeight - border.Height);

                // Проверка на пересечение
                if (!viewModel.CheckForCollision(table, newX, newY))
                {
                    table.X = newX;
                    table.Y = newY;
                }
            }
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging && sender is Border border && border.DataContext is TableModel table)
            {
                isDragging = false;
                border.ReleaseMouseCapture();

                // Финальное выравнивание при отпускании кнопки мыши
                table.X = table.SnapToGrid(table.X);
                table.Y = table.SnapToGrid(table.Y);
            }
        }

        private double Clamp(double value, double min, double max)
        {
            return value < min ? min : (value > max ? max : value);
        }
    }
}