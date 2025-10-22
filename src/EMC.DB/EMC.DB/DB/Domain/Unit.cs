using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace EMC.DB
{
    [Table("Unit")]
    public class Unit
    {
        [Key]
        public int Id { get; private set; }

        [Required]
        public string Name { get; set; } = ""; // 'mm','deg','sec' ...

        public string Symbol { get; set; }     // '㎜','°','s'
    }
}
