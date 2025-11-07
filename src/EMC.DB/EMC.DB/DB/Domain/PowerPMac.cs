
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMC.DB
{
    public class PowerPMac
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Ip { get; set; }

        public ICollection<PowerPMacMotion> MotionList { get; set; } = new List<PowerPMacMotion>();
    }
}
