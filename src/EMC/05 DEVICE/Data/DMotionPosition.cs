using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace EMC
{
    public partial class DMotionPosition : ObservableObject
    {
        public int Id { get; set; }
        [ObservableProperty] private string name;
        [ObservableProperty] private double speed;
        [ObservableProperty] private double minimumSpeed;
        [ObservableProperty] private double maximumSpeed;

        [ObservableProperty] private double location;
        [ObservableProperty] private double minimumLocation;
        [ObservableProperty] private double maximumLocation;

        [ObservableProperty] private int parentMotionId;
    }
}