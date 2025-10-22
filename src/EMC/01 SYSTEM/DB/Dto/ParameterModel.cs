using CommunityToolkit.Mvvm.ComponentModel;

namespace EMC
{
    public partial class ParameterModel : ObservableObject
    {
        [ObservableProperty] private string name = "";
        [ObservableProperty] private string value = "";
        [ObservableProperty] private string maximumValue = "";
        [ObservableProperty] private string minimumValue = "";
        [ObservableProperty] private string unit = "";
    }
}
