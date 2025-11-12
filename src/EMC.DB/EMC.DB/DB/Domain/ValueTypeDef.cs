using EPFramework.DB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMC.DB
{
    [Table("ValueType")]
    public class ValueTypeDef : IEntity
    {

        [Required]
        public string Name { get; set; } = ""; // 'INT','REAL','TEXT','BOOL' 등
    }
}
