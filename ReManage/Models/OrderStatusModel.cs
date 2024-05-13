using System.ComponentModel.DataAnnotations.Schema;

namespace ReManage.Models
{
    [Table("order_status")]
    public class OrderStatusModel
    {
        [Column("id")]
        public int id { get; set; }

        [Column("name")]
        public string name { get; set; }
    }
}