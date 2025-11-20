using CommunityToolkit.Mvvm.ComponentModel;

namespace EMC
{
    public partial class DMotionPosition : ObservableObject
    {
        [ObservableProperty] private int id;
        [ObservableProperty] private string name;
        [ObservableProperty] private double speed;
        [ObservableProperty] private double location;
        [ObservableProperty] private IMotion parentMotion;
    }
}
