using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReManage.Models
{
    [Table("refrigerator")]
    public class RefrigeratorModel
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }

        [Column("amount")]
        public int Amount { get; set; }

        [Column("date_delivered")]
        public DateTime DateDelivered { get; set; }

        [Column("shelf_life")]
        public TimeSpan ShelfLife { get; set; }

        [Column("unfreeze_time")]
        public TimeSpan? UnfreezeTime { get; set; }

        [Column("expiry_date")]
        public DateTime ExpiryDate { get; set; }
    }
}