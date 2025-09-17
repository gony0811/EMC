
using System.ComponentModel.DataAnnotations;

namespace EGGPLANT
{
    public class Error 
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Number { get; set; }

        public string Name { get; set; }

        public string Cause { get; set; }           // 원인
        public string Solution { get; set; }        // 해결방법

        public int? BuzzerId { get; set; }
    }
}
