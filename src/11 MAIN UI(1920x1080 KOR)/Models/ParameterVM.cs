using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace EGGPLANT
{
    public partial class ParameterVM : ObservableObject
    {

        public int RecipeId { get; }

        public int ParameterId { get; }

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string value;

        [ObservableProperty]
        private string maximum;

        [ObservableProperty]
        private string minimum;

        [ObservableProperty]
        private string valueType;

        [ObservableProperty]
        private string unit;

        public ParameterVM()
        {
        }

        private ParameterVM(RecipeParamDto dto)
        {
            RecipeId = dto.RecipeId;
            ParameterId = dto.ParameterId ?? 0;
            Name = dto.Name;
            Value = dto.Value;
            Maximum = dto.Maximum;
            Minimum = dto.Minimum;
            ValueType = dto.ValueType;
            Unit = dto.Unit;
        }
        public static ParameterVM of(RecipeParamDto dto)
        {
            return new ParameterVM(dto);
        }
    }
}
