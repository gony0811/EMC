using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace EMC
{
    public abstract partial class DMotion : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty] private string name;
        [ObservableProperty] private int motorNo;
        [ObservableProperty] private string controlType;
        [ObservableProperty] private bool isEnabled;

        [ObservableProperty] private int parentDeviceId; 

        // private 필드로 변경
        [ObservableProperty]
        private ObservableCollection<DMotionPosition> positionList = new ObservableCollection<DMotionPosition>();

        // private 필드로 변경
        [ObservableProperty]
        private ObservableCollection<DMotionParameter> parameterList = new ObservableCollection<DMotionParameter>();
    }
}