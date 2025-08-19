
using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

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

        [ObservableProperty]
        private bool? isMainChecked;
        [ObservableProperty]
        private bool? isParamChecked;
        [ObservableProperty]
        private bool? isUserChecked;
        [ObservableProperty]
        private bool? isLogChecked;
        [ObservableProperty]
        private bool? isErrorChecked;
        [ObservableProperty]
        private bool? isManualChecked;
        [ObservableProperty]
        private bool? isMotionChecked;
        [ObservableProperty]
        private bool? isDirectIOChecked;
        [ObservableProperty]
        private bool? isTowerLampChecked;
        [ObservableProperty]
        private ObservableCollection<string> logMessages = new ObservableCollection<string>();


        // public CSYS CSYS { get; } = App.Container?.Resolve<CSYS>();

        // 수정 코드
        public CSYS CSYS { get; } = App.Container?.Resolve<CSYS>() ?? throw new InvalidOperationException("CSYS 인스턴스를 생성할 수 없습니다. App.Container가 null이거나 등록되지 않았습니다.");


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

            Init();
        }

        private void Init()
        {
            // Initialize any additional properties or perform setup tasks here
            IsMainChecked = true;
            IsParamChecked = false;
            IsUserChecked = false;
            IsLogChecked = false;
            IsErrorChecked = false;
            IsManualChecked = false;
            IsMotionChecked = false;
            IsDirectIOChecked = false;
            IsTowerLampChecked = false;
            // Navigate to the initial page
            CSYS.GoToSub01();
            CSYS.Trace.SetTextBox(LogMessages, 100);
            CSYS.Trace.Trace("UMainViewModel", "UMainViewModel initialized successfully.");

        }



        [RelayCommand]
        private void NavigateClick(string tag)
        {
            switch(tag)
            {
                case "MAIN":
                    CSYS.GoToSub01();
                    IsMainChecked = true;
                    IsParamChecked = false;
                    IsUserChecked = false;
                    IsLogChecked = false;
                    IsErrorChecked = false;
                    IsManualChecked = false;
                    IsMotionChecked = false;
                    IsDirectIOChecked = false;
                    IsTowerLampChecked = false;
                    break;
                case "PARAM":
                    CSYS.GoToSub02();
                    IsMainChecked = false;
                    IsParamChecked = true;
                    IsUserChecked = false;
                    IsLogChecked = false;
                    IsErrorChecked = false;
                    IsManualChecked = false;
                    IsMotionChecked = false;
                    IsDirectIOChecked = false;
                    IsTowerLampChecked = false;
                    break;
                case "USER":
                    IsMainChecked = false;
                    IsParamChecked = false;
                    IsUserChecked = true;
                    IsLogChecked = false;
                    IsErrorChecked = false;
                    IsManualChecked = false;
                    IsMotionChecked = false;
                    IsDirectIOChecked = false;
                    IsTowerLampChecked = false;
                    break;
                case "LOG":
                    CSYS.GoToSub04();
                    IsMainChecked = false;
                    IsParamChecked = false;
                    IsUserChecked = false;
                    IsLogChecked = true;
                    IsErrorChecked = false;
                    IsManualChecked = false;
                    IsMotionChecked = false;
                    IsDirectIOChecked = false;
                    IsTowerLampChecked = false;
                    break;
                case "ERROR":
                    CSYS.GoToSub05();
                    IsMainChecked = false;
                    IsParamChecked = false;
                    IsUserChecked = false;
                    IsLogChecked = false;
                    IsErrorChecked = true;
                    IsManualChecked = false;
                    IsMotionChecked = false;
                    IsDirectIOChecked = false;
                    IsTowerLampChecked = false;
                    break;
                case "MANUAL":
                    IsMainChecked = false;
                    IsParamChecked = false;
                    IsUserChecked = false;
                    IsLogChecked = false;
                    IsErrorChecked = false;
                    IsManualChecked = true;
                    IsMotionChecked = false;
                    IsDirectIOChecked = false;
                    IsTowerLampChecked = false;
                    break;
                case "MOTION":
                    IsMainChecked = false;
                    IsParamChecked = false;
                    IsUserChecked = false;
                    IsLogChecked = false;
                    IsErrorChecked = false;
                    IsManualChecked = false;
                    IsMotionChecked = true;
                    IsDirectIOChecked = false;
                    IsTowerLampChecked = false;
                    break;
                case "SENSOR":
                    IsMainChecked = false;
                    IsParamChecked = false;
                    IsUserChecked = false;
                    IsLogChecked = false;
                    IsErrorChecked = false;
                    IsManualChecked = false;
                    IsMotionChecked = false;
                    IsDirectIOChecked = true;
                    IsTowerLampChecked = false;
                    break;
                case "TOWERLAMP":
                    IsMainChecked = false;
                    IsParamChecked = false;
                    IsUserChecked = false;
                    IsLogChecked = false;
                    IsErrorChecked = false;
                    IsManualChecked = false;
                    IsMotionChecked = false;
                    IsDirectIOChecked = false;
                    IsTowerLampChecked = true;
                    break;
                default:
                    throw new ArgumentException("Invalid tag for navigation");
            }
            // Navigate to the main page or perform any action needed
        }
    }
}
