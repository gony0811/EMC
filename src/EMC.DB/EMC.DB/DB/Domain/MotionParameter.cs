using EPFramework.DB;
using System.ComponentModel.DataAnnotations;

namespace EMC.DB
{
    public sealed class MotionParameter : IEntity
    {

        [Required]
        public string Name { get; set; } = "";

        [Required]
        public ValueType ValueType { get; set; }

        public int? IntValue { get; set; }
        public double? DoubleValue { get; set; }
        public bool? BoolValue { get; set; }
        public string StringValue { get; set; }

        public UnitType UnitType { get; set; }

        public MotionEntity Motion { get; set; }
        public int MotionId { get; set; }

    }
}
