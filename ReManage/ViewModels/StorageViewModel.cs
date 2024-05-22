using NodaTime;
using System;

namespace ReManage.ViewModels
{
    public class StorageViewModel : ProductViewModel
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
        public DateTime ExpiryDate { get; set; }
        public string ExpiryDateFormatted => ExpiryDate.ToString("dd.MM.yyyy");
    }
}