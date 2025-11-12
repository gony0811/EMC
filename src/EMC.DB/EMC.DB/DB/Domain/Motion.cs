using EPFramework.DB;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMC.DB
{
    public class Motion : IEntity
    {

        [Required]
        public string Name { get; set; } = "";

        public string ControlType { get; set; }

        public int DeviceId { get; set; }

        public bool IsEnabled { get; set; } = true;

         public Device ParentDevice { get; set; }

        public ICollection<MotionPosition> PositionList { get; set; } = new List<MotionPosition>();
        public ICollection<MotionParameter> ParameterList { get; set; } = new List<MotionParameter>();
    }
}
