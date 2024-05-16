using System.ComponentModel.DataAnnotations.Schema;
using ReManage.Core;

namespace ReManage.Models
{
    [Table("products")]
    public class ProductModel : ViewModelBase
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("weight")]
        public int Weight { get; set; }

        [Column("price")]
        public decimal Price { get; set; }
    }
}