using NodaTime;
using ReManage.Core;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReManage.Models
{
    [Table("refrigerator")]
    public class RefrigeratorModel : ViewModelBase
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("amount")]
        public int Amount { get; set; }

        [Column("date_delivered")]
        public DateTime DateDelivered { get; set; }

        [Column("shelf_life")]
        public Period ShelfLife { get; set; }

        [Column("unfreeze_time")]
        public Period UnfreezeTime { get; set; }
    }
}