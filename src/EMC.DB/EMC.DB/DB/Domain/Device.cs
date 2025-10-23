using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMC.DB
{

    public class Device
    {
        [Key]
        public int Id { get; private set; }

        [Required]
        [Index(nameof(Name), IsUnique = true)]
        public string Name { get; set; }

        public string Module { get; set; }

        public DataType Type { get; set; }
        public string TypeString { get; set; }


    }
}
