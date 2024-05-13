using System.ComponentModel.DataAnnotations.Schema;
using ReManage.Core;

namespace ReManage.Models
{
    [Table("ordered_dishes")]
    public class OrderedDishModel : ViewModelBase
    {
        [Column("id")]
        public int id { get; set; }

        [Column("order_id")]
        public int order_id { get; set; }

        [Column("dish_id")]
        public int dish_id { get; set; }

        [Column("amount")]
        public int amount { get; set; }
    }
}