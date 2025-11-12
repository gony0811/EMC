using EPFramework.DB;
using System.ComponentModel.DataAnnotations;

namespace EMC.DB
{
    public sealed class MotionPosition : IEntity
    {

        [Required]
        public string Name { get; set; } = "";

        public double Speed { get; set; } = 0.0;
        public double MinimumSpeed { get; set; } = 1;
        public double MaximumSpeed { get; set; } = 100;

        public double Location { get; set; } = 0.0;
        public double MinimumLocation { get; set; } = 0.0;
        public double MaximumLocation { get; set; } = 0.0;

        public int MotionId { get; set; }

        public Motion Motion { get; set; }

    }
}
