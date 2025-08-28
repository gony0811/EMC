using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EGGPLANT._12_SUB_UI
{
    public partial class AxisWaypointVM : ObservableObject
    {
        // 데이터
        [ObservableProperty] private string locationName;
        [ObservableProperty] private double position;
        [ObservableProperty] private double speed;

        // 범위(최소/최대) 저장
        [ObservableProperty] private double minPosition = 0.0;
        [ObservableProperty] private double maxPosition = 0.0;
        [ObservableProperty] private double minSpeed = 0.0;
        [ObservableProperty] private double maxSpeed = 0.0;

        [RelayCommand]
        private void Move()
        {

        }

        [RelayCommand]
        private void Apply()
        {
            // TODO : 필요에 따라 없어질수도 있음.
        }


        // 생성자

        public AxisWaypointVM() { }
        public AxisWaypointVM(string name, double pos, double spd,
                              double minPos, double maxPos, double minSpd, double maxSpd)
        {
            LocationName = name;
            MinPosition = minPos; MaxPosition = maxPos;
            MinSpeed = minSpd; MaxSpeed = maxSpd;
            Position = pos;
            Speed = spd;
        }
    }
}
