
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EGGPLANT
{
    [Table("PermissionCategory")]
    public class PermissionCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public int DisplayOrder { get; set; } = 0;

        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
        public ICollection<RoleCategoryManage> RoleManages { get; set; } = new List<RoleCategoryManage>();
    }
}
