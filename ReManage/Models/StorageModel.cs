﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReManage.Models
{
    [Table("storage")]
    public class StorageModel
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
        public TimeSpan ShelfLife { get; set; } // Предполагаем, что это TimeSpan в днях

        [Column("expiry_date")]
        public DateTime ExpiryDate { get; set; } 

        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
    }
}