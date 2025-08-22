using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EGGPLANT.Models
{
    public partial class SensorIoItemViewModel : ObservableObject
    {

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private bool isChecked;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ToggleCommand))]
        private bool isReadOnly;

        public SensorIoItemViewModel() { }

        public SensorIoItemViewModel(string name, bool isChecked = false, bool isReadOnly = false)
        {
            Name = name;
            IsChecked = isChecked;
            IsReadOnly = isReadOnly;
        }

        [RelayCommand(CanExecute = nameof(CanToggle))]
        private void Toggle()
        {
            if (IsReadOnly) return;
            //IsChecked = !IsChecked;

            // TODO: 실제 장비/서비스로 출력 전송이 필요하면 여기에서 호출
            // _ioService.WriteOutput(Name, IsChecked);
        }

        private bool CanToggle() => !IsReadOnly;

        // ReadOnly가 바뀌면 CanExecute 새로고침
        partial void OnIsCheckedChanged(bool oldValue, bool newValue)
        {
            if (!IsReadOnly)
            {
                // _ioService.WriteOutput(Name, newValue);
            }
        }
    }

}