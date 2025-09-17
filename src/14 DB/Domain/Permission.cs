

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EGGPLANT
{

    [Table("Permission")]
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string? Description { get; set; }

        public bool IsEnabled { get; set; } = false;

        public PermissionCategory? Category { get; set; }
    }
}
