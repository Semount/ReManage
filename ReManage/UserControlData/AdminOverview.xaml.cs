using System;
using System.Linq;
using System.Windows.Controls;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ReManage.Core;
using ReManage.Models;

namespace ReManage.UserControlData
{
    public partial class AdminOverview : UserControl
    {
        public PlotModel ProfitPlotModel { get; private set; }
        public PlotModel PopularDishesPlotModel { get; private set; }

        public AdminOverview()
        {
            InitializeComponent();
            DataContext = this;
            LoadData();
        }

        private void LoadData()
        {
            using (var context = new RestaurantContext())
            {
                // Получение данных о прибыли
                var profitData = context.Orders
                    .GroupBy(o => o.creation_date.Date)
                    .Select(g => new { Date = g.Key, Profit = g.Sum(o => o.price) })
                    .OrderBy(d => d.Date)
                    .ToList();

                // Создание графика прибыли
                ProfitPlotModel = new PlotModel { Title = "Прибыль по дням" };
                var lineSeries = new LineSeries { Title = "Прибыль" };
                var dateAxis = new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "dd/MM/yyyy" };
                var valueAxis = new LinearAxis { Position = AxisPosition.Left, Title = "Прибыль" };
                ProfitPlotModel.Axes.Add(dateAxis);
                ProfitPlotModel.Axes.Add(valueAxis);

                foreach (var data in profitData)
                {
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(data.Date), (double)data.Profit));
                }
                ProfitPlotModel.Series.Add(lineSeries);

                // Получение данных о популярных блюдах
                var popularDishesData = context.OrderedDishes
                    .GroupBy(od => od.dish_id)
                    .Select(g => new { DishId = g.Key, Count = g.Sum(od => od.amount) })
                    .OrderByDescending(d => d.Count)
                    .Take(10)
                    .ToList();

                // Создание графика популярных блюд
                PopularDishesPlotModel = new PlotModel { Title = "Популярные блюда" };
                var categoryAxis = new CategoryAxis { Position = AxisPosition.Left, Title = "Блюда" };
                var valueAxis2 = new LinearAxis { Position = AxisPosition.Bottom, Title = "Количество заказов" };
                PopularDishesPlotModel.Axes.Add(categoryAxis);
                PopularDishesPlotModel.Axes.Add(valueAxis2);

                var barSeries = new BarSeries { Title = "Заказы", LabelPlacement = LabelPlacement.Base, LabelFormatString = "{0}", IsStacked = false };

                foreach (var data in popularDishesData)
                {
                    var dish = context.Dishes.FirstOrDefault(d => d.Id == data.DishId);
                    barSeries.Items.Add(new BarItem { Value = data.Count });
                    categoryAxis.Labels.Add(dish.Name);
                }

                PopularDishesPlotModel.Series.Add(barSeries);
            }
        }
    }
}