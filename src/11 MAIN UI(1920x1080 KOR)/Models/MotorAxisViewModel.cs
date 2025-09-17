using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EGGPLANT.Models
{
    public partial class MotorAxisViewModel : ObservableObject
    {
        public string Axis { get; }
        [ObservableProperty] private bool home, rdy, alm, inp, nLmt, org, pLmt;
        [ObservableProperty] private bool isServoOn;
        [ObservableProperty] private double ep = 0.0, cp = 0.0;
        [ObservableProperty] private double valueTop, valueBottom;
        public IRelayCommand<bool> ToggleServoCommand { get; }
        public IRelayCommand HomeCommand { get; }

        public MotorAxisViewModel(string axis)
        {
            Axis = axis;
            ToggleServoCommand = new RelayCommand<bool>(param =>
            {
                var on = param == true;
                if (on)
                {
                    ServoOn();
                }else
                {
                    ServoOff();
                }
            });
            HomeCommand = new RelayCommand(() => {  });
        }

        private void OnToggleServo(bool on)
        {
            if (on)
            {
                // 기존 ServoOnCommand 로직
                ServoOn();
            }
            else
            {
                // 기존 ServoOff 로직
                ServoOff();
            }
            IsServoOn = on;
        }

        private void ServoOn()
        {
            // 실제 서보 ON 처리
        }
        private void ServoOff()
        {
            // 실제 서보 OFF 처리
        }
    }

}
