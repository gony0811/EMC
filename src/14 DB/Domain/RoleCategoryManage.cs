
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace EGGPLANT
{
    [Table("RoleCategoryManage")]
    public class RoleCategoryManage
    {
        public int RoleId { get; set; }

        public int CategoryId { get; set; }

        public bool CanManage { get; set; } = true;

        public Role? Role { get; set; }
        public PermissionCategory? Category { get; set; }
    }

}
