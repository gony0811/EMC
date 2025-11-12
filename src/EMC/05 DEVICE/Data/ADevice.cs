using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace EMC
{
    // <TMotion> 제네릭 제거
    public abstract partial class ADevice<TMotion> : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty] private string name;
        [ObservableProperty] private string ip;
        [ObservableProperty] private string deviceType;
        [ObservableProperty] private bool isEnabled;
        [ObservableProperty] private bool isConnected;

        [ObservableProperty]
        private ObservableCollection<TMotion> motionList = new ObservableCollection<TMotion>();

        public abstract bool Connect();
        public abstract bool Disconnect();
    }
}