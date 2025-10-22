using CommunityToolkit.Mvvm.ComponentModel;

namespace EMC
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
