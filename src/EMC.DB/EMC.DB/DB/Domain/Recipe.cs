
using EPFramework.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMC.DB
{
    [Table("Recipes")]
    public class Recipe : IEntity
    {

        [Required]
        public string Name { get; set; } = "";

        public bool IsActive { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }

        public ICollection<RecipeParam> ParamList { get; set; } = new List<RecipeParam>();
    }
}
