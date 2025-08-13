
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGGPLANT
{
    public partial class UMainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? threadText;

        [ObservableProperty]
        private string? timeText;

        [ObservableProperty]
        private string? userText;

        [ObservableProperty]
        private string? statusText;

        [ObservableProperty]
        private string? buildText;

        [ObservableProperty]
        private string? developerText;

        [ObservableProperty]
        private string? serialNumberText;

        public UMainViewModel()
        {
            // Initialize properties with default values
            ThreadText = "1ms";
            TimeText = "오전 00:00:00";
            UserText = "SERVICE ENGINEER";
            StatusText = "설비 정지 상태 입니다.(설비 초기화 필요!)";
            BuildText = "Build 0000.00.00";
            DeveloperText = "Developed by EGGPLANT";
            SerialNumberText = "SN : EGGPLANT Copyright(c) EGGPLANT Corp. All rights reserved.";
        }
    }
}
