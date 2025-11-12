

using CommunityToolkit.Mvvm.ComponentModel;

namespace EMC
{
    public partial class MotionPositionCreate : ObservableObject
    {
        [ObservableProperty] private string name;
        [ObservableProperty] private double speed;
        [ObservableProperty] private double minimumSpeed;
        [ObservableProperty] private double maximumSpeed;

        [ObservableProperty] private double location;
        [ObservableProperty] private double minimumLocation;
        [ObservableProperty] private double maximumLocation;
    }
}
