
using CommunityToolkit.Mvvm.ComponentModel;

namespace EGGPLANT.Models
{
    // 수동 조작 화면에 쓰일 데이터 모델
    public partial class ManualOperationModel : ObservableObject
    {
        // 피치
        [ObservableProperty]
        private double pitch;

        [ObservableProperty] private double xSpeed;
        [ObservableProperty] private double ySpeed;
        [ObservableProperty] private double zSpeed;

        public ManualOperationModel()
        {
            Pitch = 0.01;
            XSpeed = 1.0;
            YSpeed = 1.0;
            ZSpeed = 1.0;
        }
    }
}
