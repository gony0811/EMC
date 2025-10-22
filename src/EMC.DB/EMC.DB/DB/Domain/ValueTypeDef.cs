using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMC.DB
{
    [Table("ValueType")]
    public class ValueTypeDef
    {
        [Key]
        public int Id { get; private set; }

        [Required]
        public string Name { get; set; } = ""; // 'INT','REAL','TEXT','BOOL' 등
    }
}
