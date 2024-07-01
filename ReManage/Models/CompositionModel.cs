using System.ComponentModel.DataAnnotations.Schema;
using ReManage.Core;

namespace ReManage.Models
{
    [Table("composition")]
    public class CompositionModel
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("dish_id")]
        public int DishId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("amount")]
        public int Amount { get; set; }

        [ForeignKey("DishId")]
        public DishModel Dish { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
    }
}