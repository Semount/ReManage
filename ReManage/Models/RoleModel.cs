using System.ComponentModel.DataAnnotations.Schema;
using ReManage.Core;

namespace ReManage.Models
{
    [Table("roles")]
    public class RoleModel : ViewModelBase
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("role_name")]
        public string RoleName { get; set; }
    }
}