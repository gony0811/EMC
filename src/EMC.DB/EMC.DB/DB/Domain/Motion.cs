using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMC.DB
{
    public class Motion
    {
        [Key]
        public int Id { get; private set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        public MotionAxis Axis { get; set; }
        public ICollection<MotionPosition> Positions { get; set; } = new List<MotionPosition>();
    }
}
