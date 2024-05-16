using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ReManage.Core;

namespace ReManage.Models
{
    [Table("orders")]
    public class OrderModel : ViewModelBase
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Column("employee_id")]
        public int employee_id { get; set; }

        [Column("status_id")]
        public int status_id { get; set; }

        [Column("price")]
        public decimal price { get; set; }

        [Column("creation_date")]
        public DateTime creation_date { get; set; }

        [Column("creation_time")]
        public TimeSpan creation_time { get; set; }

        [Column("serving_time")]
        public TimeSpan serving_time { get; set; }

        [Column("comment")]
        public string comment { get; set; }


        // Преобразование времени в UTC
        public void SetCreationDateToUtc()
        {
            creation_date = DateTime.SpecifyKind(creation_date, DateTimeKind.Utc);
        }
    }
}

