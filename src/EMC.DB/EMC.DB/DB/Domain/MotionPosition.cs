using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EMC.DB
{
    public sealed class MotionPosition
    {
        [Key]
        public int Id { get; private set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        [Required]
        public int MotionId { get; set; }

        public Motion Motion { get; set; }

        public double CurrentLocation { get; set; } = 0.0;
        public double MinimumLocation { get; set; } = 0.0;
        public double MaximumLocation { get; set; } = 0.0;

        public int CurrentSpeed { get; set; } = 1;
        public int MinimumSpeed { get; set; } = 1;
        public int MaximumSpeed { get; set; } = 100;
    }
}
