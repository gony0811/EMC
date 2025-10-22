﻿
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMC.DB
{
    [Table("RecipeParam")]
    public class RecipeParam
    {
        [Key]
        public int Id { get; set; } // 스키마상 단일 PK

        public int RecipeId { get; set; }


        [Required(ErrorMessage = "파라미터 이름은 필수입니다.")]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Value값은 필수입니다.")]
        public string Value { get; set; } = "";

        public string Maximum { get; set; }
        public string Minimum { get; set; }

        public int ValueTypeId { get; set; }
        public int UnitId { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public Recipe Recipe { get; set; }
        public ValueTypeDef ValueType { get; set; }
        public Unit Unit { get; set; }

        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // 문자열을 숫자로 운영하신다면 파싱 후 비교(가능하면 decimal? 컬럼을 권장)
            if (!string.IsNullOrWhiteSpace(Minimum) && !string.IsNullOrWhiteSpace(Maximum))
            {
                if (decimal.TryParse(Minimum, out var min) &&
                    decimal.TryParse(Maximum, out var max) &&
                    min > max)
                {
                    yield return new ValidationResult(
                        "최소 값이 최대 값보다 클 수 없습니다.",
                        new[] { nameof(Minimum), nameof(Maximum) });
                }
            }
        }
    }
}
