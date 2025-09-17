using System.ComponentModel.DataAnnotations;

namespace EGGPLANT
{
    public class Buzzer 
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
