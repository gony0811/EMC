using CommunityToolkit.Mvvm.ComponentModel;
using EMC.DB;

namespace EMC
{
    public partial class ParameterCellViewModel : ObservableObject
    {
        public int Id { get; }

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayValue))]   
        private object value;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayValue))]  
        private bool readOnly = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayValue))]   
        private string unit = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private ValueType valueType;

        public string DisplayValue =>
            ReadOnly
                ? $"{Value}{(string.IsNullOrWhiteSpace(Unit) ? "" : " " + Unit)}"
                : Value?.ToString() ?? "";

        public ParameterCellViewModel(
            int id,
            string name,
            object value,
            bool readOnly = false,
            string unit = "",
            ValueType valueType = ValueType.String,
            string description = "")
        {
            Id = id;
            Name = name;
            Value = value;
            ReadOnly = readOnly;
            Unit = unit;
            ValueType = valueType;
            Description = description;
        }
    }
}
