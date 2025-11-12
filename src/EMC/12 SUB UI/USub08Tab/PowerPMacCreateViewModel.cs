using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace EMC
{
    public partial class PowerPMacCreateViewModel : ObservableObject
    {
        [ObservableProperty] private string name;
        [ObservableProperty] private string ip;

        public PowerPmacDevice? Result { get; private set; }

        [RelayCommand]
        public void Confirm(Window window)
        {

            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("이름을 입력하세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Ip))
            {
                MessageBox.Show("IP 주소를 입력하세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ✅ DTO 생성
            Result = new PowerPmacDevice
            {
                Ip = Ip.Trim(),
                DeviceType = DeviceType.PowerPmac.ToString(),
                Name = Name,
                IsConnected = false,
            };

            window.DialogResult = true;
            window.Close();
        }

        [RelayCommand]
        public void Cancel(Window window)
        {
            window.DialogResult = false;
            window.Close();
        }
    }
}
