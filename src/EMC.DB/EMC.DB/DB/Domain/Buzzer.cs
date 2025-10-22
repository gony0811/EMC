using System.ComponentModel.DataAnnotations;

namespace EMC.DB
{
    public class Buzzer 
    {
        [Key]
        public int Id { get; private set; }

        public string Name { get; set; }
    }
}
