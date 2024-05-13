using System.ComponentModel.DataAnnotations.Schema;
using ReManage.Core;

namespace ReManage.Models
{
    [Table("composition")]
    public class CompositionModel : ViewModelBase
    {
        [Column("id")]
        public int id { get; set; }

        [Column("dish_id")]
        public int dish_id { get; set; }

        [Column("product_id")]
        public int product_id { get; set; }

        [Column("amount")]
        public int amount { get; set; }
    }
}