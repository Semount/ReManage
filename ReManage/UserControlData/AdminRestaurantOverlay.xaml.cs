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

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid grid && grid.DataContext is TableModel table)
            {
                isDragging = true;
                var mousePosition = e.GetPosition(grid);
                mouseOffset = new Point(mousePosition.X, mousePosition.Y);
                grid.CaptureMouse();
            }
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && sender is Grid grid && grid.DataContext is TableModel table)
            {
                var mousePosition = e.GetPosition(canvas);

                // Вычисляем новые координаты с учетом смещения мыши
                double newX = mousePosition.X - mouseOffset.X;
                double newY = mousePosition.Y - mouseOffset.Y;

                // Ограничиваем координаты, чтобы не выходить за пределы Canvas
                newX = Clamp(newX, 0, canvas.ActualWidth - grid.Width);
                newY = Clamp(newY, 0, canvas.ActualHeight - grid.Height);

                // Проверка на пересечение
                if (!viewModel.CheckForCollision(table, newX, newY))
                {
                    table.X = newX;
                    table.Y = newY;
                }
            }
        }

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging && sender is Grid grid && grid.DataContext is TableModel table)
            {
                isDragging = false;
                grid.ReleaseMouseCapture();

                // Финальное выравнивание при отпускании кнопки мыши
                table.X = table.SnapToGrid(table.X);
                table.Y = table.SnapToGrid(table.Y);
            }
        }

        private void Grid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // Дополнительная проверка на null для контекстного меню
            if (sender is Grid grid && grid.ContextMenu != null)
            {
                grid.ContextMenu.DataContext = grid.DataContext;
            }
        }

        private double Clamp(double value, double min, double max)
        {
            return value < min ? min : (value > max ? max : value);
        }
    }
}