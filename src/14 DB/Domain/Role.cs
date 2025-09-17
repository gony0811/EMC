using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EGGPLANT
{
    [Table("Roles")]
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(200)]
        public string? Description { get; set; }

        [Required, MaxLength(200)]
        public string Password { get; set; } = "";

        public bool IsActive { get; set; } = true;


        public ICollection<RoleCategoryManage> CategoryManages { get; set; } = new List<RoleCategoryManage>();
    }
}
