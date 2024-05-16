using System.ComponentModel.DataAnnotations.Schema;
using ReManage.Core;

namespace ReManage.Models
{
    [Table("employees")]
    public class EmployeeModel : ViewModelBase
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("login")]
        public string Login { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("surname")]
        public string Surname { get; set; }

        [ForeignKey("RoleId")]
        public RoleModel Role { get; set; }
    }
}