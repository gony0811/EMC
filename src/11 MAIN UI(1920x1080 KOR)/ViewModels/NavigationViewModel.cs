using CommunityToolkit.Mvvm.ComponentModel;

namespace EGGPLANT
{
    public partial class NavigationViewModel : ObservableObject
    {


        // 화면 활성화 여부 뷰 모델 
        [ObservableProperty] private bool mainEnabled = true;

        [ObservableProperty] private bool parameterEnabled = true;

        [ObservableProperty] private bool userEnabled = true;

        [ObservableProperty] private bool logEnabled = true;

        [ObservableProperty] private bool errorEnabled = true;

        [ObservableProperty] private bool manualEnabled = true;

        [ObservableProperty] private bool motorEnabled = true;

        [ObservableProperty] private bool sensorEnabled = true;

        [ObservableProperty] private bool lampEnabled = true;

        private readonly Dictionary<string, Action<bool>> _permSetters;

        public NavigationViewModel()
        {

            _permSetters = new()
            {
                ["MAIN"] = v => MainEnabled = v,
                ["PARAMETER"] = v => ParameterEnabled = v,
                ["USER"] = v => UserEnabled = v,
                ["LOG"] = v => LogEnabled = v,
                ["ERROR"] = v => ErrorEnabled = v,
                ["MANUAL"] = v => ManualEnabled = v,
                ["MOTOR"] = v => MotorEnabled = v,
                ["SENSOR"] = v => SensorEnabled = v,
                ["LAMP"] = v => LampEnabled = v,
            };
        }

        public async Task LoadNavigation(CancellationToken ct = default)
        {

            SetEnabled(false);
        }

        public void ApplyScreens(IReadOnlyCollection<string> allowed)
        {
            // 일단 모두 false
            SetEnabled(false);

            // 허용된 것만 true
            foreach (var kv in _permSetters)
                kv.Value(allowed.Contains(kv.Key));
        }

        public void SetEnabled(bool onOff)
        {
            MainEnabled = onOff;
            ParameterEnabled = onOff;
            UserEnabled = onOff;
            LogEnabled = onOff;
            ErrorEnabled = onOff;
            ManualEnabled = onOff;
            MotorEnabled = onOff;
            SensorEnabled = onOff;
            LampEnabled = onOff;
        }
    }
}
