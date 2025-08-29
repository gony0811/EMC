using CommunityToolkit.Mvvm.ComponentModel;
using EGGPLANT;

namespace EGGPLANT
{
    public enum Axis { X, Y, Z }

    public partial class MotorStateStore : ObservableObject
    {
        [ObservableProperty] private MotorStateControlVM x = new() { Title = "X축" };
        [ObservableProperty] private MotorStateControlVM y = new() { Title = "Y축" };
        [ObservableProperty] private MotorStateControlVM z = new() { Title = "Z축" };

        public MotorStateStore()
        {
            // x Motors 초기화
            X.Title = "X";
            X.MotorStates.Add(new StateCellVM("HOME", State.Offline));
            X.MotorStates.Add(new StateCellVM("READY", State.Offline));
            X.MotorStates.Add(new StateCellVM("ALARM", State.Offline));
            X.MotorStates.Add(new StateCellVM("INP", State.Offline));

            X.MotorStates.Add(new StateCellVM("N-LMT", State.Offline));
            X.MotorStates.Add(new StateCellVM("P-LMT", State.Offline));
            X.MotorStates.Add(new StateCellVM("ORG", State.Offline));
            X.MotorStates.Add(new StateCellVM("SERVO", State.Offline));


            // Y Motors 초기화
            Y.Title = "Y";
            Y.MotorStates.Add(new StateCellVM("HOME", State.Offline));
            Y.MotorStates.Add(new StateCellVM("READY", State.Offline));
            Y.MotorStates.Add(new StateCellVM("ALARM", State.Offline));
            Y.MotorStates.Add(new StateCellVM("INP", State.Offline));

            Y.MotorStates.Add(new StateCellVM("N-LMT", State.Offline));
            Y.MotorStates.Add(new StateCellVM("P-LMT", State.Offline));
            Y.MotorStates.Add(new StateCellVM("ORG", State.Offline));
            Y.MotorStates.Add(new StateCellVM("SERVO", State.Offline));

            // Z Motors 초기화
            Z.Title = "Z";
            Z.MotorStates.Add(new StateCellVM("HOME", State.Offline));
            Z.MotorStates.Add(new StateCellVM("READY", State.Offline));
            Z.MotorStates.Add(new StateCellVM("ALARM", State.Offline));
            Z.MotorStates.Add(new StateCellVM("INP", State.Offline));

            Z.MotorStates.Add(new StateCellVM("N-LMT", State.Offline));
            Z.MotorStates.Add(new StateCellVM("P-LMT", State.Offline));
            Z.MotorStates.Add(new StateCellVM("ORG", State.Offline));
            Z.MotorStates.Add(new StateCellVM("SERVO", State.Offline));

        }
        // 장치 이벤트 등 백그라운드에서 들어오면 UI 스레드로 합류
        public void UpdateAxis(Axis axis, Action<MotorStateControlVM> apply)
        {
            var target = axis switch { Axis.X => X, Axis.Y => Y, Axis.Z => Z, _ => X };
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                apply(target);
            else
                System.Windows.Application.Current.Dispatcher.Invoke(() => apply(target));
        }
    }
}
