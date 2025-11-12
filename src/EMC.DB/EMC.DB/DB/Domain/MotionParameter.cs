using EPFramework.DB;
using System.ComponentModel.DataAnnotations;

namespace EMC.DB
{
    public sealed class MotionParameter : IEntity
    {

        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string ValueType { get; set; } = "string";       /// double | int | string | bool

        public int IntValue { get; set; }
        public double DoubleValue { get; set; }
        public bool BoolValue { get; set; }
        public string StringValue { get; set; }

        public string Unit { get; set; }

        public Motion Motion { get; set; }
        public int MotionId { get; set; }

    }
}
