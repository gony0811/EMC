using EPFramework.DB;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMC.DB
{

    public class Device : IEntity
    {

        [Required]
        public string Name { get; set; } = "";

        public string Ip { get; set; }

        public string DeviceType { get; set; }

        public bool IsEnabled { get; set; } = true;

        public ICollection<Motion> MotionList { get; set; } = new List<Motion>();

    }
}
