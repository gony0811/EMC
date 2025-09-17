using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EGGPLANT
{
    [Table("ValueType")]
    public class ValueTypeDef
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = ""; // 'INT','REAL','TEXT','BOOL' 등
    }
}
