
using CommunityToolkit.Mvvm.ComponentModel;
using EGGPLANT._11_MAIN_UI_1920x1080_KOR_.Models;
using System.Collections.ObjectModel;

namespace EGGPLANT._11_MAIN_UI_1920x1080_KOR_.ViewModels
{
    public partial class USubViewModel02 : ObservableObject
    {
        // 현재 디바이스, 공용 파라미터, 기본 파라미터 선택, 디바이스 목록, 아이템 이름, 
        [ObservableProperty]
        private string currentDevice;

        // 공용 & 기본 파라미터 선택
        [ObservableProperty]
        private bool parameterType = true; // False: 공용, True: 기본 파라미터

        [ObservableProperty]
        private ObservableCollection<DeviceItem> devices = new ObservableCollection<DeviceItem>();

        [ObservableProperty]
        private DeviceItem selectedDevice;

        [ObservableProperty]
        private DeviceItem activeDevice; // 현재 사용중 기기 (옵션)

        [ObservableProperty]
        private ObservableCollection<ParameterModel> items = new ObservableCollection<ParameterModel>();


        public USubViewModel02()
        {
            // 샘플 데이터 Device 목록
            Devices.Add(new DeviceItem { Index = 1, Name = "Camera #1", IsInUse = true });
            Devices.Add(new DeviceItem { Index = 2, Name = "Camera #2", IsInUse = false});

            // 샘플데이터 파라미터 아이템
            Items.Add(new ParameterModel { Name = "STAGE MOVE 안정화 시간", Value = "152.4", Unit = "ms" });
            Items.Add(new ParameterModel { Name = "LDS 측정 응답 TIME OUT 시간", Value = "200", Unit = "ms" });
            Items.Add(new ParameterModel { Name = "CROSS 오차", Value = "20.00", Unit = "um" });

            foreach (var d in Devices) HookItem(d);
            Devices.CollectionChanged += (_, e) =>
            {
                if (e.NewItems != null) foreach (DeviceItem d in e.NewItems) HookItem(d);
            };
        }

        private void HookItem(DeviceItem d)
        {
            d.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(DeviceItem.IsInUse) && d.IsInUse)
                {
                    foreach (var other in Devices)
                        if (!ReferenceEquals(other, d) && other.IsInUse)
                            other.IsInUse = false;

                    ActiveDevice = d;     // (옵션) 추적용
                    SelectedDevice = d;     // UI 선택 동기화
                }
            };
        }
    }

    
}
