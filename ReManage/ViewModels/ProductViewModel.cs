using System;
using ReManage.Core;

namespace ReManage.ViewModels
{
    public class ProductViewModel : ViewModelBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
        public DateTime DateDelivered { get; set; }
        public DateTime ExpirationDate { get; set; }

        // Форматирование ExpirationDate в читаемый вид
        public string ExpirationDateFormatted => ExpirationDate.ToString("yyyy-MM-dd");
    }
}