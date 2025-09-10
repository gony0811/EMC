namespace EGGPLANT
{
    public sealed class RecipeDto
    {
        public int RecipeId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string CreatedAt { get; set; }

        public string UpdatedAt { get; set; }
    }

    public sealed class RecipeParamDto
    {
        public int RecipeId { get; set; }
        public int? ParameterId { get; set; }
        public string Name { get; set; }

        public string Value { get; set; }
        public string Maximum { get; set; }
        public string Minimum { get; set; }
        public string ValueType { get; set; }
        public string Unit { get; set; }

        public RecipeParamDto()
        {
        }

        public RecipeParamDto(int recipeId, string name, string value, string maximum, string minimum, string valueType, string unit)
        {
            RecipeId = recipeId;
            Name = name;
            Value = value;
            Maximum = maximum;
            Minimum = minimum;
            ValueType = valueType;
            Unit = unit;
        }
    }

    
}
