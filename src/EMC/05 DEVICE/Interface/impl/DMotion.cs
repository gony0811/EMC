

using CommunityToolkit.Mvvm.ComponentModel;
using EMC.DB;
using System.Collections.ObjectModel;

namespace EMC
{
    public partial class DMotion : ObservableObject, IMotion
    {
        [ObservableProperty] private int id;
        [ObservableProperty] private string name;
        [ObservableProperty] private int motorNo;

        [ObservableProperty] private UnitType unit;
        [ObservableProperty] private double isEnabled;

        [ObservableProperty] private double currentSpeed;
        [ObservableProperty] private double limitMinSpeed;
        [ObservableProperty] private double limitMaxSpeed;

        [ObservableProperty] private double currentPosition;
        [ObservableProperty] private double limitMinPosition;
        [ObservableProperty] private double limitMaxPosition;

        [ObservableProperty] private IMotionDevice device;

        [ObservableProperty] public ObservableCollection<DMotionParameter> parameterList = new ObservableCollection<DMotionParameter>();
        [ObservableProperty] public ObservableCollection<DMotionPosition> positionList = new ObservableCollection<DMotionPosition>();

    }
}
