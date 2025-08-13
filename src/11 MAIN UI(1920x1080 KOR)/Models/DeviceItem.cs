using CommunityToolkit.Mvvm.ComponentModel;

namespace EGGPLANT._11_MAIN_UI_1920x1080_KOR_.Models
{
    public partial class DeviceItem : ObservableObject
    {
        [ObservableProperty]
        private int index;

        [ObservableProperty]
        private string name;

        [ObservableProperty] 
        private bool isInUse;
    }
}
