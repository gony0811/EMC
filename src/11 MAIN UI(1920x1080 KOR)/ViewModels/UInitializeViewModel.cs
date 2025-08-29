using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
