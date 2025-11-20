using System.ComponentModel.DataAnnotations;

namespace EPFramework.DB
{
    public abstract class IEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
