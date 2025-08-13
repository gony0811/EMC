using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGGPLANT._11_MAIN_UI_1920x1080_KOR_.ViewModels
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
