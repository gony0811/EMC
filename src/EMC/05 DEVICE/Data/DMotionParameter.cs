using CommunityToolkit.Mvvm.ComponentModel;
namespace EMC
{
    public partial class DMotionParameter : ObservableObject
    {
        public int Id { get; set; }
        [ObservableProperty] private string name;
        [ObservableProperty] private string valueType;
        [ObservableProperty] private int intValue;
        [ObservableProperty] private double doubleValue;
        [ObservableProperty] private bool boolValue;
        [ObservableProperty] private string stringValue;
        [ObservableProperty] private string unit;

        [ObservableProperty] private int parentMotionId;
    }
}