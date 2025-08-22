using CommunityToolkit.Mvvm.ComponentModel;

namespace EGGPLANT.ViewModels
{
    public partial class UInitializeViewModel: ObservableObject
    {
        [ObservableProperty]
        private int progress;

        [ObservableProperty]
        private string currentTitle;

        [ObservableProperty]
        private string status; // "진행 중...", "완료"
    }
}
