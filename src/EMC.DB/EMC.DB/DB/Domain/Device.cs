using System.ComponentModel.DataAnnotations;

namespace EMC.DB
{

    public class Device
    {
        [Key]
        public int Id { get; private set; }

        [Required]
        public string Name { get; set; }

        public string Module { get; set; }

        public DataType Type { get; set; }
        public string TypeString { get; set; }


    }
}
