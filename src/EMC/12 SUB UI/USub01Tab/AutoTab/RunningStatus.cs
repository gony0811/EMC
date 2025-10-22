using CommunityToolkit.Mvvm.ComponentModel;
using EPFramework.IoC;

namespace EMC
{
    [ViewModel(Lifetime.Scoped)]
    public partial class RunningStatus : ObservableObject
    {
        public TimeRangeViewModel RunningTimeRange { get; }
        public TimeRangeViewModel LoadingTimeRange { get; }


        public RunningStatus()
        {
            RunningTimeRange = new TimeRangeViewModel();
            LoadingTimeRange = new TimeRangeViewModel();
        }
    }
}
