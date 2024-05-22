using NodaTime; 
using ReManage.ViewModels;
using System;

public class RefrigeratorViewModel : ProductViewModel
{
    public Period ShelfLife { get; set; }

    // Форматируем Period для отображения в UI
    public string ShelfLifeFormatted
    {
        get
        {
            if (ShelfLife == null)
                return "Не указан";
            return $"{ShelfLife.Years} лет, {ShelfLife.Months} месяцев, {ShelfLife.Days} дней";
        }
    }

    public TimeSpan UnfreezeTime { get; set; }
    public string UnfreezeTimeFormatted => $"{UnfreezeTime.Hours:D2}:{UnfreezeTime.Minutes:D2}:{UnfreezeTime.Seconds:D2}";
}